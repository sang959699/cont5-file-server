import { Component, OnInit, Injectable } from '@angular/core';

import { LoginService } from '../../services/login.service';
import { Router } from '@angular/router';
import { UntypedFormBuilder, UntypedFormGroup, Validators } from '@angular/forms';
import { Observable, Subscription } from 'rxjs';
import { first } from 'rxjs/operators';
import { FileService } from 'src/app/services/file.service';
import { SessionService } from 'src/app/services/session.service';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss']
})

@Injectable()
export class LoginComponent implements OnInit {
  loginForm: UntypedFormGroup;
  userName: string;
  password: string;
  isError: boolean;

  constructor(private router: Router, private loginService: LoginService, private formBuilder: UntypedFormBuilder, private fileService: FileService, private sessionService: SessionService) { }

  ngOnInit() {
    if (this.sessionService.cont5Token) {
      this.loginService.validateToken().pipe(first()).subscribe(res => {
        if (res) {
          this.router.navigate(['/fileList']);
        }
      })
    }

    this.loginForm = this.formBuilder.group({
      userName: ['', Validators.required],
      password: ['', Validators.required],
    });
  }

  authenticate(userName, password) {
    if (this.loginForm.invalid && !(userName || password)) {
      this.showError();
      return;
    }
    this.loginService.authenticate(userName, password).pipe(first())
    .subscribe(s => {
      if (!s.token) {
        this.showError();
      }
    });
  }

  showError() {
    this.isError = true;
    setTimeout(() => {
      this.isError = false;
    }, 2000);
  }
}
