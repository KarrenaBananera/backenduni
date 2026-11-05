import { Component, computed, inject, signal } from '@angular/core';
import { ProductImageComponent } from "../../../shared/components/product-image-component/product-image-component";
import { ProductListComponent } from "../../products/product-list-component/product-list-component";
import { orderListComponent } from "../products-list-component/products-list-component";
import { RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { CartService } from '../../../services/cart-service';

@Component({
  selector: 'app-order-page-component',
  imports: [orderListComponent, RouterLink, FormsModule],
  templateUrl: './order-page-component.html',
  styleUrl: './order-page-component.css',
})
export class OrderPageComponent {
  promoCode: string = ''; 
  cart = inject(CartService);
  protected isPromoApplied = computed( () => this.cart.discountPercent() != 0);

  onApplyClick()
  {
    if (this.promoCode === "BUILD10")
    {
      this.cart.discountPercent.set(0.1);
    }
  }
}
