const app = angular.module('collegeApp', []);

//app.config(function ($httpProvider) {
//    $httpProvider.defaults.transformRequest = function (data) {
//        if (data === undefined) {
//            return data;
//        }
//        return JSON.stringify(data);
//    }
//});

const studentController = (scope, http) => {

    scope.students = [];

    http.get("/Home/GetData").then((data) => {
        console.log(data);
    });
};