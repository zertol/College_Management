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

    scope.teachersEdit = [];

    scope.subject = {
        subjectId: -1,
        courseId: null,
        teacherId: null,
        title: "",
    };

    scope.errors = {};
    scope.modalTitle = "Add Subject";
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
                scope.modalTitle="Add Subject"
            });
        });

    };

    scope.addSubject = (event) => {
        event.preventDefault();
        scope.errors = {};

        if (scope.subject.subjectId == -1) {
            scope.errors.subjectId = "Subject ID must be greater than 0.";
            return;
        }

        if (!scope.subject.courseId) {
            scope.errors.courseId = "Please link a course to this subject.";
            return;
        }

        if ((scope.subjectsCollection.filter(x => x.SubjectId == scope.subject.subjectId)).length > 0) {
            scope.errors.subjectId = "Subject exists! Please select a different ID.";
            return;
        }

        http.post("/Subjects/Create", scope.subject).then((data) => {
            const { TeacherId, Enrollments, ...rest } = data.data;
            var mappedSubject = {
                ...rest,
                Teacher: scope.subject.teacherId
                    ? { Id: scope.subject.teacherId, Name: scope.teachersCourses.Teachers.filter(t => t.Id == scope.subject.teacherId)[0].Name }
                    : { Id: -1, Name: "N/A" },
                Students: [],
                NbStudents: 0
            };
            console.log(mappedSubject);
            scope.subjectsCollection.push(mappedSubject);
            $("#pop-modal").modal('hide');
        });
    };

    scope.editSubject = ($data, row) => {
        console.log($data);
        var d = $q.defer();
        var rowEdit = {
            ...$data,
            SubjectId: row.SubjectId,
            CourseId: row.CourseId
        };

        http.post("/Subjects/Edit", rowEdit)
            .then(data => {
                const res = data.data;
                if (res.result.toLowerCase() == "ok") {
                    d.resolve();
                }
                else {
                    scope.errors.global = res.message;
                    d.reject(res.message);
                }

            },
                error => {
                    scope.errors.global = "Unable to process the request."
                    d.reject("Unable to process the request.");
                });
        return d.promise;
    };

    scope.removeSubject = id => {
        if (confirm("Are you sure you want to delete this record?")) {
            http.post("/Subjects/Delete/" + id).then(data => {
                const res = data.data;
                if (res.result.toLowerCase() == "ok") {
                    scope.subjectsCollection = scope.subjectsCollection.filter(subject => subject.SubjectId !== id);
                }
                else {
                    scope.errors.global = res.message;
                }
            },
                error => {
                    scope.errors.global = error;
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

    scope.loadTeachersCourses = () => {
        http.get("/Subjects/GetTeachersCourses").then((data) => {
            scope.teachersCourses = {
                ...scope.teachersCourses,
                Courses: data.data.Courses,
                Teachers: data.data.Teachers
            };
        });
    }

    //scope.loadTeachersEdit = () => {
    //    scope.teachersEdit = [...scope.teachersCourses.Teachers];
    //};

    scope.loadTeachersCourses();
};


const teachersController = (scope, http, $q, $compile) => {

    scope.teachersCollection = [];
    scope.subjectsCollection = [{SubjectId: null, Title: "None"}];
    scope.studentDetails = [];

    scope.teachersCourses = {
        Teachers: [],
        Courses: []
    };

    scope.teacher = {
        subjectId: null,
        name: "",
        birthday: "",
        salary: 0
    };

    scope.errors = {};
    scope.modalTitle = "Add Teacher";



    scope.cancelEdit = () => {
        scope.error = "";
    };

    scope.addTeacher = (event) => {
        event.preventDefault();
        scope.errors = {};

        //if (!scope.teacher.subjectId) {
        //    scope.errors.subjectId = "Please link/create an unassigned Subject.";
        //    return;
        //}

        if (scope.teacher.salary == 0) {
            scope.errors.salary = "Salary must be greater than 0!";
            return;
        }

        http.post("/Teachers/Create", scope.teacher).then((data) => {

            console.log(data.data);

            scope.teachersCollection.push(data.data);
            $("#pop-modal").modal('hide');
        });
    };

    scope.editteacher = ($data, row) => {
        var d = $q.defer();
        var rowEdit = {
            ...$data,
            teacherId: row.teacherId,
            CourseId: row.CourseId
        };

        http.post("/teachers/Edit", rowEdit)
            .then(data => {
                const res = data.data;
                if (res.result.toLowerCase() == "ok") {
                    d.resolve();
                }
                else {
                    scope.errors.global = res.message;
                    d.reject(res.message);
                }

            },
                error => {
                    scope.errors.global = "Unable to process the request."
                    d.reject("Unable to process the request.");
                });
        return d.promise;
    };

    scope.removeteacher = id => {
        if (confirm("Are you sure you want to delete this record?")) {
            http.post("/teachers/Delete/" + id).then(data => {
                const res = data.data;
                if (res.result.toLowerCase() == "ok") {
                    scope.teachersCollection = scope.teachersCollection.filter(teacher => teacher.teacherId !== id);
                }
                else {
                    scope.errors.global = res.message;
                }
            },
                error => {
                    scope.errors.global = error;
                });
        }

    };

    http.get("/Teachers/GetData").then((data) => {
        scope.teachersCollection = data.data
    });

    http.get("/Teachers/GetSubjects").then((data) => {
        scope.subjectsCollection = [...scope.subjectsCollection, ...data.data];
    });
};