import { Component, effect, inject, signal } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { ProductService } from '../../../services/product-service';
import { toSignal } from '@angular/core/rxjs-interop';
import { map } from 'rxjs';
import { CommonModule } from '@angular/common';
import { ProductImageComponent } from "../../../shared/components/product-image-component/product-image-component";
import { SpecificationComponent } from "../specification-component/specification-component";
import { StarsComponent } from "../../../shared/components/stars-component/stars-component";
import { CartService } from '../../../services/cart-service';
import { ToastrService } from 'ngx-toastr';
import { Product } from '../../../shared/models/product';

@Component({
  selector: 'app-product-details-page',
  imports: [CommonModule, RouterLink, ProductImageComponent, SpecificationComponent, StarsComponent],
  templateUrl: './product-details-page.html',
  styleUrl: './product-details-page.css',
})
export class ProductDetailsPage {
   route = inject(ActivatedRoute);
   productService = inject(ProductService);
   cart = inject(CartService);
   toastr = inject(ToastrService);

   amount = signal(1);
   product = signal<Product | null>(null);
   clickIncreace()
   {
    this.amount.update(x => x+1);
   }

   clickDecrease()
   {
    if (this.amount() != 1)
      this.amount.update(x => x-1);
   }

   onAddToCart()
   {
    this.cart.addProducts(this.product()!, this.amount())
    this.toastr.success("",`Added ${this.amount()} ${this.product()!.name} to cart`);
   }

   idSignal = toSignal(
    this.route.paramMap.pipe(map(params => parseInt(params.get('id') ?? '0'))),
    { initialValue: undefined }
  );

  constructor() {
    effect(() => {
      const id = this.idSignal();
      if (!id) {
        this.product.set(null);
        return;
      }

      this.productService.getById(id).subscribe({
        next: (item) => this.product.set(item),
        error: () => this.product.set(null),
      });
    });
  }
}
