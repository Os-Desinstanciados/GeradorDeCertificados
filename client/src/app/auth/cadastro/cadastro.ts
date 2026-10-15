import { Component } from '@angular/core';
import { PaginaAutenticacao } from '../pagina-autenticacao';
import { LucideArrowRight } from '@lucide/angular';
import { RouterLink } from '@angular/router';

@Component({
  imports: [PaginaAutenticacao, LucideArrowRight, RouterLink],
  selector: 'app-cadastro',
  templateUrl: './cadastro.html',
})
export class Cadastro {}
