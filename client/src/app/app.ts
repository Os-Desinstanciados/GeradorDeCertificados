import { Component, signal } from '@angular/core';
import { RouterOutlet, RouterLink } from '@angular/router';
import { LucideAArrowDown, LucideLayoutArrowRight } from '@lucide/angular';

@Component({
  imports: [LucideLayoutArrowRight, RouterLink],
  selector: 'app-root',
  styleUrl: './app.css',
  templateUrl: './app.html',
})
export class App {
  protected readonly title = signal('gerador-certificados');
}
