import { Component, signal } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { ProductListComponent } from './pages/products/product-list-component/product-list-component';
import { FilterComponent } from './pages/products/filter-component/filter-component';
import { HeaderComponent } from "./shared/components/header-component/header-component";
import { FooterComponent } from "./shared/components/footer-component/footer-component";

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, HeaderComponent, FooterComponent],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App {
  protected readonly title = signal('siteUni');
}
