angular.module("umbraco").controller("Limbo.Umbraco.Vimeo.Video", function ($scope, $element, $timeout, vimeoService) {

    const vm = this;

    vm.value = null;

    let rawVideoData = null;

    // Gets information about the video of the entered URL/embed code
    vm.getVideo = function () {

        const source = vm.value && vm.value.source ? vm.value.source.trim() : null;

        vm.error = null;

        if (source) {

            vm.loading = true;
            vm.updateUI();

            vimeoService.getVideo(source).then(function (res) {

                // Update the property value
                vm.value.credentials = res.data.credentials;
                vm.value.parameters = res.data.parameters;

                // As Umbraco/JSON.net will corrupt any timestamps in the JSON, we need to store it as serialized
                vm.value.video = { _data: angular.toJson(res.data.video) };
                rawVideoData = res.data.video;

                // Update the property value
                vm.sync();

                vm.loading = false;
                vm.updateUI();

            }, function (res) {

                vm.loading = false;

                vm.embed = false;
                rawVideoData = null;

                vm.value = { source: vm.value.source };

                // Update the property value
                vm.sync();

                vm.updateUI();

                if (res.data.message) {
                    vm.error = { message: res.data.message };
                }

            });

        } else {

            vm.embed = false;
            rawVideoData = null;

            vm.value = null;

            // Update the property value
            vm.sync();

            vm.updateUI();

        }

    };

    vm.sync = function () {

        // In order to reset the property value, we need to set the value to an empty string rather than null as
        // Umbraco otherwise will save a string with the value "null" instead of an actual null value
        $scope.model.value = vm.value?.source ? vm.value : "";

    };

    // Triggered by the UI when the user changes the URL/embed code
    vm.updated = function () {
        vm.sync();
        vm.getVideo();
    };

    vm.refresh = function () {
        vm.getVideo();
    };

    // Updates the video information for the UI
    vm.updateUI = function () {

        const embed = vm.value && vm.value.source && vm.value.source.indexOf("<") >= 0;

        if (embed !== vm.embed) {
            vm.embed = embed;
            const el = $element[0].querySelector(embed ? "textarea" : "input");
            if (el) {
                // Add a bit delay so the element is visible before we try to focus it
                $timeout(function () {
                    el.focus();
                }, 20);
            }
        }

        if (!rawVideoData) {
            vm.videoId = null;
            vm.title = null;
            vm.description = null;
            vm.duration = null;
            vm.thumbnail = null;
            return;
        }

        vm.videoId = rawVideoData.uri.split("/")[2].split(":")[0];
        vm.title = rawVideoData.name;
        vm.description = rawVideoData.description;
        vm.thumbnail = vimeoService.getThumbnail(rawVideoData);

        vm.duration = rawVideoData.duration;

    };

    function init() {

        if (!$scope.model.config) $scope.model.config = {};
        $scope.model.hideLabel = $scope.model.config.hideLabel === true;

        if (!$scope.model.value || $scope.model.value === "null") {
            $scope.model.value = null;
            return;
        }

        if (!$scope.model.value) return;
        if (!$scope.model.value.video) return;
        if (!$scope.model.value.video._data) return;

        // Umbraco has some an annoying behaviour when saving null values, so we need to work keep a shadow model to
        // work around this issue
        vm.value = $scope.model.value;

        // Get the Vimeo video data from the "_data" property (necessary due to Umbraco/JSON.net issue)
        rawVideoData = angular.fromJson($scope.model.value.video._data);

        vm.updateUI();

    }

    init();

});