import { KeyValuePipe } from '@angular/common';
import { Component, input, signal } from '@angular/core';

@Component({
  selector: 'app-specification-component',
  imports: [KeyValuePipe],
  templateUrl: './specification-component.html',
  styleUrl: './specification-component.css',
})
export class SpecificationComponent {
  fields = input.required<Record<string,string>>();
  expanded = signal(false);

  protected toggle(): void {
    this.expanded.update(prev => !prev);
  }
}
