import { Component } from '@angular/core';
import { RouterLink, RouterOutlet } from '@angular/router';
import { LucideArrowRight, LucideAward, LucideSparkles } from '@lucide/angular';

@Component({
  imports: [LucideAward, LucideSparkles, LucideArrowRight, RouterLink, RouterOutlet],
  selector: 'app-home',
  templateUrl: './home.html',
})
export class Home {}
