import { Component, computed, inject, signal } from '@angular/core';
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


   product = computed(() => {
    const id = this.idSignal();
    const products = this.productService.products();
    if (!id || products.length === 0) return undefined; 
    return products.find(p => p.id === id) ?? null; 
  });
}
