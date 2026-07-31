import { computed, inject } from '@angular/core';

import {
  signalStore,
  withComputed,
  withMethods,
  patchState,
  withState,
} from '@ngrx/signals';


import {
  withEntities,
  setAllEntities,
  updateEntity,
} from '@ngrx/signals/entities';


import { rxMethod } from '@ngrx/signals/rxjs-interop';


import {
  pipe,
  concatMap,
  tap,
  catchError,
  EMPTY,
} from 'rxjs';


import { EnrollmentService } from '../services/enrollment';
import { Enrollment } from '../models/enrollment.model';



export const EnrollmentStore = signalStore(

  {
    providedIn: 'root'
  },


  // Simple store state
  withState({

    isLoading: false,

    error: null as string | null

  }),



  // Entity collection
  withEntities<Enrollment>(),



  // Derived state
  withComputed((store) => ({

    pendingCount: computed(

      () =>
        store
          .entities()
          .filter(
            enrollment => enrollment.status === 'Pending'
          )
          .length

    )

  })),



  // Store methods
  withMethods(

    (
      store,
      api = inject(EnrollmentService)
    ) => ({



      // ============================
      // Load all enrollments
      // ============================
      loadEnrollments: rxMethod<void>(

        pipe(

          tap(() => {

            patchState(
              store,
              {
                isLoading: true,
                error: null
              }
            );

          }),



          concatMap(() =>


            api.getAll().pipe(


              tap((rows) => {


                patchState(

                  store,

                  setAllEntities(rows),

                  {
                    isLoading:false
                  }

                );


              }),



              catchError((err) => {


                patchState(

                  store,

                  {
                    isLoading:false,
                    error:err.message
                  }

                );


                return EMPTY;


              })


            )


          )


        )

      ),





      // ============================
      // Approve enrollment
      // ============================
      approveEnrollment: rxMethod<string>(


        pipe(


          // Optimistic update
          tap((id) => {


            patchState(

              store,

              updateEntity({

                id,

                changes:{

                  status:'Approved'

                }

              })

            );


          }),





          concatMap((id) =>


            api.approve(id).pipe(


              catchError((err)=>{


                // Rollback if server rejects

                patchState(

                  store,

                  updateEntity({

                    id,

                    changes:{

                      status:'Pending'

                    }

                  })

                );



                patchState(

                  store,

                  {

                    error:
                    'Server rejected the approval. Check enrollment constraints.'

                  }

                );



                return EMPTY;


              })


            )


          )


        )


      )



    })


  )


);