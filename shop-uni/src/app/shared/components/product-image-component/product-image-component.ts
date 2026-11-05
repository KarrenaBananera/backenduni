import { Component, input } from '@angular/core';

@Component({
  selector: 'app-product-image-component',
  imports: [],
  templateUrl: './product-image-component.html',
  styleUrl: './product-image-component.css',
})
export class ProductImageComponent {
  images = input.required<string[]>();
  currentIndex = 0;

  get currentImage(): string | undefined {
    return this.images()[this.currentIndex];
  }

  prev(){
    this.currentIndex =
      (this.currentIndex - 1 + this.images().length) % this.images().length;
  }

  next() {
    this.currentIndex = (this.currentIndex + 1) % this.images().length;
  }

  selectImage(index: number){
    if (index >= 0 && index < this.images().length) {
      this.currentIndex = index;
    }
  }
}
