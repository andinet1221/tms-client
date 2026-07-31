import { Component, EventEmitter, Input, Output } from '@angular/core';
import { Course } from '../../models/course.model';


@Component({
  selector: 'tms-course-card',
  standalone: true,
  templateUrl: './course-card.component.html',
  styleUrl: './course-card.component.css'
})
export class CourseCardComponent {


  @Input() course!: Course;


  @Output() enrollClicked = 
    new EventEmitter<Course>();


  enroll(){

    this.enrollClicked.emit(this.course);

  }


}