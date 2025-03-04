angular.module("umbraco").controller("Limbo.Umbraco.Vimeo.ButtonList", function ($scope) {

    const vm = this;

    // Get the query string of the view URL
    const urlParts = $scope.model.view.split("?");
    const urlQuery = new URLSearchParams(urlParts.length === 1 ? "" : urlParts[1]);
    const type = urlQuery.get("type");

    vm.options = [];

    vm.select = function (option) {
        $scope.model.value = option.alias;
    };

    if (type === "showTitle" || type === "showByLine" || type === "showPortrait") {

        vm.options = [
            { alias: null, label: "Inherit" },
            { alias: true, label: "Enabled" },
            { alias: false, label: "Disabled" }
        ];

        if (!$scope.model.value && $scope.model.value !== false) $scope.model.value = vm.options[0].alias;

    } else {

        vm.options = [
            { alias: "inherit", label: "Inherit" },
            { alias: "enabled", label: "Enabled" },
            { alias: "disabled", label: "Disabled" }
        ];

        if (!$scope.model.value) $scope.model.value = vm.options[0].alias;

    }

});