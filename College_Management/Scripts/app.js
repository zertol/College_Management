//Functions
const ToJavaScriptDate = value => {
    var pattern = /Date\(([^)]+)\)/;
    var results = pattern.exec(value);
    var dt = new Date(parseFloat(results[1]));
    return (dt.getMonth() + 1) + "/" + dt.getDate() + "/" + dt.getFullYear();
};

//Modules
const app = angular.module('collegeApp', ['smart-table', '720kb.datepicker', 'xeditable','ngSanitize']);

app.run(editableOptions => editableOptions.theme = 'bs4');

//Directives
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


//Controllers
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
        scope.modalTitle = "Student Details";
    
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
            scope.subjectsCollection.push(mappedSubject);
            $("#pop-modal").modal('hide');
        });
    };

    scope.editSubject = ($data, row) => {
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

    scope.selectTeacher = teacher => {
        var selected = scope.teachersCourses.Teachers.filter(t => t.Id === teacher.Id);

        return selected.length ? selected[0].Name : "N/A";
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
    scope.loadTeachersCourses();
};

const teachersController = (scope, http, $q, $compile) => {

    scope.teachersCollection = [];
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

        if (scope.teacher.salary == 0) {
            scope.errors.salary = "Salary must be greater than 0!";
            return;
        }

        http.post("/Teachers/Create", scope.teacher).then((data) => {
            var mappedTeacher = {
                ...data.data,
                Birthday: new Date(ToJavaScriptDate(data.data["Birthday"]))
            };
            scope.teachersCollection.push(mappedTeacher);
            $("#pop-modal").modal('hide');
        });
    };

    scope.editTeacher = ($data, row) => {
        var d = $q.defer();
        var rowEdit = {
            ...$data,
            Id: row.Id,
            Birthday: row.Birthday
        };

        http.post("/Teachers/Edit", rowEdit)
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

    scope.removeTeacher = id => {
        if (confirm("Are you sure you want to delete this record?")) {
            http.post("/Teachers/Delete/" + id).then(data => {
                const res = data.data;
                if (res.result.toLowerCase() == "ok") {
                    scope.teachersCollection = scope.teachersCollection.filter(teacher => teacher.Id !== id);
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
        scope.teachersCollection = data.data.map(x => {
            return {
                ...x,
                Birthday: new Date(ToJavaScriptDate(x["Birthday"]))
            };
        });
    });

};

const coursesController = (scope, http, $q, $compile) => {

    scope.coursesCollection = [];
    scope.subjectsCollection = [{ SubjectId: null, Title: "None" }];
    scope.studentDetails = [];

    scope.coursesCourses = {
        courses: [],
        Courses: []
    };

    scope.course = {
        courseId: -1,
        title: ""
    };

    scope.errors = {};
    scope.modalTitle = "Add Course";



    scope.cancelEdit = () => {
        scope.error = "";
    };

    scope.addCourse = (event) => {
        event.preventDefault();
        scope.errors = {};

        if (scope.course.courseId == -1) {
            scope.errors.courseId = "Course ID must be greater than 0.";
            return;
        }

        if ((scope.coursesCollection.filter(x => x.CourseId == scope.course.courseId)).length > 0) {
            scope.errors.subjectId = "Course exists! Please select a different ID.";
            return;
        }

        http.post("/Courses/Create", scope.course).then((data) => {
            console.log(data.data);
            scope.coursesCollection.push(data.data);
            $("#pop-modal").modal('hide');
        });
    };

    scope.editCourse = ($data, row) => {
        var d = $q.defer();
        var rowEdit = {
            ...$data,
            CourseId: row.CourseId,
        };

        http.post("/Courses/Edit", rowEdit)
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

    scope.removeCourse = id => {
        if (confirm("Are you sure you want to delete this record?")) {
            http.post("/Courses/Delete/" + id).then(data => {
                const res = data.data;
                if (res.result.toLowerCase() == "ok") {
                    scope.coursesCollection = scope.coursesCollection.filter(course => course.CourseId !== id);
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

    http.get("/Courses/GetData").then((data) => {
        scope.coursesCollection = data.data;
        //scope.coursesCollection = data.data.map(x => {
        //    return {
        //        ...x,
        //        Birthday: new Date(ToJavaScriptDate(x["Birthday"]))
        //    };
        //});
    });

    //http.get("/courses/GetSubjects").then((data) => {
    //    scope.subjectsCollection = [...scope.subjectsCollection, ...data.data];
    //});
};

const enrollmentsController = (scope, http, $q, $compile) => {

    scope.enrollmentsCollection = [];

    scope.prerequisites = {
        Subjects: [],
        Students: []
    };

    scope.enrollment = {
        subjectId: null,
        studentId: null,
        grade: null
    };

    scope.errors = {};
    scope.modalTitle = "Add Enrollment";

    scope.cancelEdit = () => {
        scope.error = "";
    };

    scope.addEnrollment = (event) => {
        event.preventDefault();
        scope.errors = {};

        if (!scope.enrollment.subjectId) {
            scope.errors.subjectId = "Please select a subject!";
            return;
        }

        if (!scope.enrollment.studentId) {
            scope.errors.studentId = "Please select a student!";
            return;
        }
        http.post("/Enrollments/Create", scope.enrollment).then((data) => {
            scope.enrollmentsCollection.push(data.data);
            $("#pop-modal").modal('hide');
        });
    };

    scope.editEnrollment = ($data, row) => {
        var d = $q.defer();

        var rowEdit = {
            ...$data,
            Id: row.Id,
        };

        http.post("/Enrollments/Edit", rowEdit)
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

    scope.removeEnrollment = id => {
        if (confirm("Are you sure you want to delete this record?")) {
            http.post("/enrollments/Delete/" + id).then(data => {
                const res = data.data;
                if (res.result.toLowerCase() == "ok") {
                    scope.enrollmentsCollection = scope.enrollmentsCollection.filter(enrollment => enrollment.Id !== id);
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

    http.get("/Enrollments/GetData").then((data) => {
        scope.enrollmentsCollection = data.data;
    });

    http.get("/Enrollments/GetPrerequisites").then((data) => {
        scope.prerequisites = data.data;
    });
};