import { CommonModule } from '@angular/common';
import { Component, inject } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../core/services/auth.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  template: `
    <div class="container py-5">
      <div class="row justify-content-center">
        <div class="col-md-7 col-lg-5">
          <div class="card shadow-sm border-0">
            <div class="card-body p-4">
              <h2 class="mb-3">Login account</h2>
              <div class="alert alert-danger" *ngIf="errorMessage">{{ errorMessage }}</div>
              <form [formGroup]="form" (ngSubmit)="submit()">
                <div class="mb-3">
                  <label class="form-label">Email</label>
                  <input class="form-control" type="email" formControlName="email" />
                </div>
                <div class="mb-3">
                  <label class="form-label">Password</label>
                  <input class="form-control" type="password" formControlName="password" />
                </div>
                <button class="btn btn-brand w-100" [disabled]="form.invalid || loading">
                  {{ loading ? 'Signing in...' : 'Sign in' }}
                </button>
              </form>
              <p class="mt-3 mb-0">No account? <a routerLink="/register">Sign up</a></p>
            </div>
          </div>
        </div>
      </div>
    </div>
  `
})
export class LoginComponent {
  private readonly fb = inject(FormBuilder);
  loading = false;
  errorMessage = '';
  readonly form = this.fb.nonNullable.group({
    email: ['', [Validators.required, Validators.email]],
    password: ['', Validators.required]
  });

  constructor(
    private readonly authService: AuthService,
    private readonly router: Router
  ) {}

  submit(): void {
    if (this.form.invalid) return;
    this.errorMessage = '';
    this.loading = true;
    this.authService.login(this.form.getRawValue()).subscribe({
      next: () => void this.router.navigateByUrl('/app/dashboard'),
      error: () => {
        this.loading = false;
        this.errorMessage = 'Login failed. Check tenant header, credentials, and role permissions.';
      }
    });
  }
}
