import { Temporal } from "@js-temporal/polyfill";

// ==========================================================
// Exercise 2 - Domain Models
// ==========================================================

import { Student } from "./models/student.models";

console.log("========== Exercise 2 ==========");

const student: Student = {
    id: "STU-001",
    name: "Hana Tadesse",
    enrollmentDate: Temporal.Now.instant(),
};

// Safe access to optional GPA
console.log(student.gpa?.toFixed(2) ?? "Not yet graded");

// Uncomment to test readonly
// student.id = "STU-999";



// ==========================================================
// Exercise 3 - Type Guard and parseStudent
// ==========================================================

import { isStudent, parseStudent } from "./models/student.models";

console.log("\n========== Exercise 3 ==========");

function processStudent(raw: unknown) {
    if (isStudent(raw)) {
        const gpaDisplay = raw.gpa?.toFixed(2) ?? "Not yet graded";
        console.log(`Student ${raw.name} GPA: ${gpaDisplay}`);
    } else {
        console.log("Invalid student data received");
    }
}

// Type Guard Test
processStudent({
    id: "STU-002",
    name: "Hana",
    gpa: 3.7
});

processStudent(42);

// parseStudent Test

console.log("\nValid Student:");

console.log(
    parseStudent({
        id: "STU-003",
        name: "Dawit"
    })
);

console.log("\nInvalid Student:");

try {
    console.log(
        parseStudent({
            id: 42,
            name: "Test"
        })
    );
} catch (error) {
    console.error((error as Error).message);
}



// ==========================================================
// Exercise 4 - Assessment Union
// ==========================================================

import {
    AssessmentItem,
    calculateGrade
} from "./models/assesment.model";

console.log("\n========== Exercise 4 ==========");

const quiz: AssessmentItem = {
    id: "QUIZ-001",
    kind: "quiz",
    title: "SQL Basics",
    correctAnswers: 8,
    totalQuestions: 10
};

const lab: AssessmentItem = {
    id: "LAB-001",
    kind: "lab",
    title: "REST API Project",
    functionalityScore: 85,
    codeQualityScore: 90
};

console.log(`Quiz Grade: ${calculateGrade(quiz)}%`);
console.log(`Lab Grade: ${calculateGrade(lab)}%`);

// Uncomment to test readonly
// quiz.id = "QUIZ-999";



// ==========================================================
// Exercise 5A - Enrollment Lifecycle
// ==========================================================

import {
    EnrollmentStatus,
    describeEnrollment
} from "./models/enrollment.models";

console.log("\n========== Exercise 5A ==========");

const pending: EnrollmentStatus = {
    status: "PENDING",
    requestedAt: Temporal.Now.instant(),
    studentId: "STU-001",
    courseId: "CRS-101"
};

console.log(describeEnrollment(pending));



// ==========================================================
// Exercise 5B - Course Lifecycle
// ==========================================================

import {
    Course,
    CourseStatus,
    describeCourse
} from "./models/course.models";

console.log("\n========== Exercise 5B ==========");

const activeCourse: CourseStatus = {
    status: "ACTIVE",
    enrolledCount: 28,
    startDate: Temporal.PlainDate.from("2026-09-01")
};

console.log(describeCourse(activeCourse));



// ==========================================================
// Exercise 6 - Generic API Response
// ==========================================================

import {
    ApiResponse,
    renderResponse
} from "./models/api-response.model";

console.log("\n========== Exercise 6 ==========");

// Student Response

const studentResponse: ApiResponse<Student> = {
    status: "success",
    data: {
        id: "STU-004",
        name: "Dawit Bekele",
        enrollmentDate: Temporal.Now.instant(),
        gpa: 3.4
    },
    fetchedAt: Temporal.Now.instant()
};

console.log(
    renderResponse(
        studentResponse,
        (s) => `${s.name} GPA: ${s.gpa ?? "N/A"}`
    )
);

// Course Response

const courseResponse: ApiResponse<Course[]> = {
    status: "success",
    data: [
        {
            id: "CRS-101",
            title: "Web Development Fundamentals",
            capacity: 30,
            startDate: Temporal.PlainDate.from("2026-09-01")
        }
    ],
    fetchedAt: Temporal.Now.instant()
};

console.log(
    renderResponse(
        courseResponse,
        (courses) => courses.map(c => c.title).join(", ")
    )
);



// ==========================================================
// Exercise 7 - Temporal
// ==========================================================

console.log("\n========== Exercise 7 ==========");

// Current UTC time

const approvedAt = Temporal.Now.instant();

console.log(`Approved at (UTC): ${approvedAt}`);

// Different Time Zones

const addisTime =
    approvedAt.toZonedDateTimeISO("Africa/Addis_Ababa");

const londonTime =
    approvedAt.toZonedDateTimeISO("Europe/London");

console.log(`Addis Time : ${addisTime.toPlainTime()}`);
console.log(`London Time: ${londonTime.toPlainTime()}`);

// Days until Course Starts

const courseStart =
    Temporal.PlainDate.from("2026-09-01");

const today =
    Temporal.Now.plainDateISO();

const daysUntilStart =
    today.until(courseStart).total({ unit: "days" });

console.log(
    `${Math.floor(daysUntilStart)} days until course starts`
);

// Assignment Deadline

const deadline =
    Temporal.PlainDate.from("2026-12-15");

const remaining =
    today.until(deadline);

console.log(
    `${remaining.total({ unit: "days" })} days until assignment is due`
);