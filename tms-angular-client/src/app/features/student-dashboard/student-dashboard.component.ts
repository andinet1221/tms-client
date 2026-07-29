import { Component, signal } from '@angular/core';
import { CourseCardComponent } from '../../ui/course-card/course-card.component';
import { Course } from '../../models/course.model';
import { RouterLink } from '@angular/router';
import { inject } from '@angular/core';
import { rxResource } from '@angular/core/rxjs-interop';
import { CourseService } from '../../services/course.service';

@Component({
  selector: 'app-student-dashboard',
  standalone: true,
  imports: [
    CourseCardComponent
  ],
  templateUrl: './student-dashboard.component.html',
  styleUrl: './student-dashboard.component.scss'
})
export class StudentDashboardComponent {


  // Stores the last clicked enrollment request
  selectedCourse = signal<Course | null>(null);

  private api = inject(CourseService);

coursesResource = rxResource({
  stream: () => this.api.getAll(),
});

  handleEnroll(course: Course){

    this.selectedCourse.set(course);


    console.log(
      "Enrollment requested for:",
      course.title
    );

  }


}