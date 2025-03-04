angular.module("umbraco").controller("Limbo.Umbraco.Vimeo.Color", function ($scope, angularHelper) {

    const vm = this;

    vm.change = function (color) {
        angularHelper.safeApply($scope, function () {
            $scope.model.value = color ? color.toHexString().trimStart("#") : null;
        });
    }

    vm.select = function (value) {

        if (!value) value == "inherit";

        if (value === "custom") value = vm.color ?? "00adef";

        if (value.length === 6) {

            vm.color = value;

            $scope.model.value = value;

            vm.options = {
                type: "color",
                color: $scope.model.value,
                allowEmpty: true,
                showAlpha: false
            };

        } else {

            $scope.model.value = value;

        }

    }

    vm.select($scope.model.value);

});