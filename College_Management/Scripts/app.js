const ToJavaScriptDate = value => {
    var pattern = /Date\(([^)]+)\)/;
    var results = pattern.exec(value);
    var dt = new Date(parseFloat(results[1]));
    return (dt.getMonth() + 1) + "/" + dt.getDate() + "/" + dt.getFullYear();
};

const app = angular.module('collegeApp', ['smart-table', '720kb.datepicker', 'xeditable','ngSanitize']);

app.run(editableOptions => editableOptions.theme = 'bs4');

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


const studentsController = (scope, http, $q) => {

    scope.studentsCollection = [];
    scope.student = {
        name: "",
        birthday: "",
        regNumber: 123
    };

    scope.error = "";


    scope.cancelEdit = () => {
        scope.error = "";
    };

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

    scope.editStudent = ($data, row) => {
        var d = $q.defer();

        var rowEdit = {
            ...$data,
            Id: row.Id
        };

        http.post("/Students/Edit", rowEdit)
            .then(data => {
                const res = data.data;
                if (res.result.toLowerCase() == "ok") {
                    d.resolve();
                }
                else {
                    scope.error = res.message;
                    d.reject(res.message);
                }
                
            },
            error => {
                scope.error = "Unable to process the request."
                d.reject("Unable to process the request.");
            });
        return d.promise;
    };

    scope.removeStudent = id => {
        if (confirm("Are you sure you want to delete this record?")) {
            http.post("/Students/Delete/" + id).then(data => {
                const res = data.data;
                if (res.result.toLowerCase() == "ok") {
                    scope.studentsCollection = scope.studentsCollection.filter(student => student.Id !== id);
                }
                else {
                    scope.error = res.message;
                }
            },
            error => {
                scope.error = error;
            });
        }
        
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


const subjectsController = (scope, http, $q, $compile) => {

    scope.subjectsCollection = [];
    scope.studentDetails = [];

    scope.teachersCourses = {
        Teachers: [],
        Courses: []
    };

    scope.dataSelect = {
        teacherId: null,
        courseId: null
    }

    scope.subject = {
        name: "",
        birthday: "",
        regNumber: 123
    };

    scope.error = "";
    scope.modalTitle = "";
    scope.modalContent = "";
    scope.detailsShown = false;

    

    scope.cancelEdit = () => {
        scope.error = "";
    };

    scope.showDetails = (row) => {

        scope.studentDetails = row.Students;
        scope.detailsShown = true;
    
        $("#pop-modal").modal('show');
        $("#pop-modal").on('hidden.bs.modal', () => {
            scope.$apply(() => {
                scope.detailsShown = false;
            });
        });

    };

    scope.addSubject = (event) => {
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

    scope.editStudent = ($data, row) => {
        var d = $q.defer();

        var rowEdit = {
            ...$data,
            Id: row.Id
        };

        http.post("/Students/Edit", rowEdit)
            .then(data => {
                const res = data.data;
                if (res.result.toLowerCase() == "ok") {
                    d.resolve();
                }
                else {
                    scope.error = res.message;
                    d.reject(res.message);
                }

            },
                error => {
                    scope.error = "Unable to process the request."
                    d.reject("Unable to process the request.");
                });
        return d.promise;
    };

    scope.removeStudent = id => {
        if (confirm("Are you sure you want to delete this record?")) {
            http.post("/Students/Delete/" + id).then(data => {
                const res = data.data;
                if (res.result.toLowerCase() == "ok") {
                    scope.studentsCollection = scope.studentsCollection.filter(student => student.Id !== id);
                }
                else {
                    scope.error = res.message;
                }
            },
                error => {
                    scope.error = error;
                });
        }

    };

    http.get("/Subjects/GetData").then((data) => {
        scope.subjectsCollection = data.data.map(x => {
            return {
                ...x,
                NbStudents: x.Students.length
            };
        });
    });

    http.get("/Subjects/GetTeachersCourses").then((data) => {
        scope.teachersCourses = {
            ...scope.teachersCourses,
            Courses : data.data.Courses,
            Teachers : data.data.Teachers
        };
    });
};