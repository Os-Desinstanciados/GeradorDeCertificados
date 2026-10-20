import { Component } from '@angular/core';
import { RouterLink } from '@angular/router';
import { LucideArrowRight } from '@lucide/angular';
import { AuthShell } from '../shell/auth-shell';

@Component({
  imports: [AuthShell, LucideArrowRight, RouterLink, AuthShell],
  selector: 'app-login',
  templateUrl: './login.html',
})
export class Login {}
