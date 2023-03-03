(function (angular: ng.IAngularStatic) {
    var app: ng.IModule = angular.module("cdnApp", ["ngRoute"]);
    
    interface IHomeControllerScope extends ng.IScope {
        displayName?: string;
        userName: string;
        isAdmin: boolean;
    }
    class HomeController {
        constructor($scope: IHomeControllerScope) {
        }
    }

    app.config([
        "$routeProvider",
        "$locationProvider",
        function ($routeProvider: ng.route.IRouteProvider, $locationProvider: ng.ILocationProvider) {
            $routeProvider
                .when('/home', {
                    templateUrl: "home.htm",
                    controller: HomeController,
                    controllerAs: "controller"
                })
                .when('/', {
                    redirectTo: "/home"
                });
            // configure html5 to get links working on jsfiddle
            $locationProvider.html5Mode(true);
        },
    ]);
})(window.angular);
