angular.module("umbraco").controller("Limbo.Umbraco.Vimeo.Video", function ($scope, $element, $timeout, limboVideoService, vimeoService) {

    const vm = this;

    let rawVideoData = null;

    // Gets information about the video of the entered URL/embed code
    vm.getVideo = function () {

        const source = $scope.model.value && $scope.model.value.source ? $scope.model.value.source.trim() : null;
        
        if (source) {

            vm.loading = true;
            vm.updateUI();

            vimeoService.getVideo(source).then(function (res) {

                // Update the credentials
                $scope.model.value.credentials = res.data.credentials;
                $scope.model.value.embed = res.data.embed;

                // As Umbraco/JSON.net will corrupt any timestamps in the JSON, we need to store it as serialized
                $scope.model.value.video = { _data: angular.toJson(res.data.video) };
                rawVideoData = res.data.video;

                vm.loading = false;
                vm.updateUI();

            }, function () {



            });

        } else {

            vm.embed = false;
            rawVideoData = null;

            $scope.model.value = null;

            vm.updateUI();

        }

    };

    // Triggered by the UI when the user changes the URL/embed code
    vm.updated = function () {
        vm.getVideo();
    };

    vm.refresh = function () {
        vm.getVideo();
    };

    // Updates the video information for the UI
    vm.updateUI = function () {

        const embed = $scope.model.value && $scope.model.value.source && $scope.model.value.source.indexOf("<") >= 0;

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
            vm.duration = null;
            vm.thumbnail = null;
            return;
        }

        vm.videoId = rawVideoData.uri.split("/")[2];
        vm.title = rawVideoData.name;
        vm.thumbnail = vimeoService.getThumbnail(rawVideoData);

        limboVideoService.getDuration(rawVideoData, function (value) {
            vm.duration = value;
        });

    };

    function init() {

        if (!$scope.model.value) return;
        if (!$scope.model.value.video) return;
        if (!$scope.model.value.video._data) return;

        // Get the Vimeo video data from the "_data" property (necessary due to Umbraco/JSON.net issue)
        rawVideoData = angular.fromJson($scope.model.value.video._data);

        vm.updateUI();

    }

    init();

});