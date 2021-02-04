const ToJavaScriptDate = value => {
    var pattern = /Date\(([^)]+)\)/;
    var results = pattern.exec(value);
    var dt = new Date(parseFloat(results[1]));
    return (dt.getMonth() + 1) + "/" + dt.getDate() + "/" + dt.getFullYear();
};

const app = angular.module('collegeApp', ['smart-table','720kb.datepicker']);

app.directive('popModal', () => {
    return {
        restrict: 'E',
        replace: true,
        transclude: true,
        link: function (scope, element, attrs) {
            scope.title = attrs.title;
        },
        template: '<div class="modal" id="pop-modal"><div class="modal-dialog"><div class="modal-content"><div class="modal-header"><h4 class="modal-title">{{title}}</h4><button type="button" class="close" data-dismiss="modal">&times;</button></div><div class="modal-body ng-transclude"></div></div></div></div>'
    };
});

//app.config(function ($httpProvider) {
//    $httpProvider.defaults.transformRequest = function (data) {
//        if (data === undefined) {
//            return data;
//        }
//        return JSON.stringify(data);
//    }
//});

const studentsController = (scope, http) => {

    scope.studentsCollection = [];
    scope.student = {
        name: "",
        birthday: "",
        regNumber:123
    }

    scope.showDetails = (elId) => {
        $("#" + elId).toggleClass("in");
    };

    scope.addStudent = (event) => {
        event.preventDefault();
        http.post("/Students/Create", scope.student).then((data) => {
            var mappedStudent = {
                ...data.data,
                Birthday: new Date(ToJavaScriptDate(data.data["Birthday"]))
            };

            scope.studentsCollection.push(mappedStudent);
            $("#pop-modal").modal('hide');
        });

    };

    http.get("/Students/GetData").then((data) => {
        scope.studentsCollection = data.data.map(x => {
            return {
                ...x,
                Birthday: new Date(ToJavaScriptDate(x["Birthday"]))
            };
        });
    });
};