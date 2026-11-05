import { Component, input, output } from '@angular/core';
import { Product } from '../../models/product';
import { NgOptimizedImage } from '@angular/common';
import { StarsComponent } from '../stars-component/stars-component';

@Component({
  selector: 'app-card-component',
  imports: [NgOptimizedImage, StarsComponent],
  templateUrl: './card-component.html',
  styleUrl: './card-component.css',
})
export class CardComponent {
  product = input.required<Product>();

  addToCart = output<Product>();
  productClick = output<Product>();

  onCardClick(): void {
    this.productClick.emit(this.product());
  }

  onAddToCartClick(event: MouseEvent): void {
    event.stopPropagation();
    this.addToCart.emit(this.product());
  }

}
