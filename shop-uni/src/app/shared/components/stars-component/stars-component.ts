import { Component, input } from '@angular/core';

@Component({
  selector: 'app-stars-component',
  imports: [],
  templateUrl: './stars-component.html',
  styleUrl: './stars-component.css',
})
export class StarsComponent {
  rating = input.required<number>();
  size = input<number>(20);

  getStarTypes(rating: number | undefined): string[] {
    const stars : string[] = [];
    if (rating == undefined)
      return stars;

    for (let i = 0; i < 5; i++) {
      if (rating >= i + 1) {
        stars.push('full');
      } else if (rating >= i + 0.5) {
        stars.push('half');
      } else {
        stars.push('empty');
      }
    }
    return stars;
  }
}
