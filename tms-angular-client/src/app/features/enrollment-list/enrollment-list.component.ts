import { Component, inject, OnInit } from '@angular/core';
import { EnrollmentStore } from '../../store/enrollment.store';


@Component({
  selector: 'tms-enrollment-list',
  standalone: true,
  templateUrl: './enrollment-list.component.html',
  styleUrl: './enrollment-list.component.css'
})
export class EnrollmentListComponent implements OnInit {


  // Inject the singleton store
  store = inject(EnrollmentStore);



  ngOnInit() {

    // Load data when component starts
    this.store.loadEnrollments();

  }



  onApprove(id: string) {

    // Call optimistic approval
    this.store.approveEnrollment(id);

  }


}