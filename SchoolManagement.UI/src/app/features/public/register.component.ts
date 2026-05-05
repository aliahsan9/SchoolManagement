import { CommonModule } from '@angular/common';
import { Component, inject } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../core/services/auth.service';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  template: `
    <div class="container py-5">
      <div class="row justify-content-center">
        <div class="col-lg-7">
          <div class="card shadow-sm border-0">
            <div class="card-body p-4">
              <h2 class="mb-3">Signup now</h2>
              <div class="alert alert-danger" *ngIf="errorMessage">{{ errorMessage }}</div>
              <form [formGroup]="form" (ngSubmit)="submit()" class="row g-3">
                <div class="col-md-6"><input class="form-control" placeholder="First name" formControlName="firstName" /></div>
                <div class="col-md-6"><input class="form-control" placeholder="Last name" formControlName="lastName" /></div>
                <div class="col-md-6"><input class="form-control" placeholder="Email" formControlName="email" /></div>
                <div class="col-md-6"><input class="form-control" placeholder="Phone number" formControlName="phoneNumber" /></div>
                <div class="col-md-6"><input type="password" class="form-control" placeholder="Password" formControlName="password" /></div>
                <div class="col-md-6">
                  <select class="form-select" formControlName="roleName">
                    <option value="Admin">Admin</option>
                    <option value="Teacher">Teacher</option>
                    <option value="Student">Student</option>
                  </select>
                </div>
                <div class="col-12"><button class="btn btn-brand w-100" [disabled]="form.invalid || loading">{{ loading ? 'Creating...' : 'Create Account' }}</button></div>
              </form>
              <p class="mt-3 mb-0">Already registered? <a routerLink="/login">Login</a></p>
            </div>
          </div>
        </div>
      </div>
    </div>
  `
})
export class RegisterComponent {
  private readonly fb = inject(FormBuilder);
  loading = false;
  errorMessage = '';
  readonly form = this.fb.nonNullable.group({
    firstName: ['', Validators.required],
    lastName: ['', Validators.required],
    email: ['', [Validators.required, Validators.email]],
    phoneNumber: [''],
    password: ['', Validators.required],
    roleName: ['Admin', Validators.required]
  });

  constructor(
    private readonly authService: AuthService,
    private readonly router: Router
  ) {}

  submit(): void {
    if (this.form.invalid) return;
    this.errorMessage = '';
    this.loading = true;
    const value = this.form.getRawValue();
    this.authService.register({ ...value, phoneNumber: value.phoneNumber || null }).subscribe({
      next: () => void this.router.navigateByUrl('/app/dashboard'),
      error: () => {
        this.loading = false;
        this.errorMessage = 'Registration failed. Please verify required fields and try again.';
      }
    });
  }
}
