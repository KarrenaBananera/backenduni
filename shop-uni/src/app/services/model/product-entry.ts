import { signal } from "@angular/core";
import { Product } from "../../shared/models/product";

export class productEntry 
{
    quantity = signal(0);
    constructor (public product : Product){}
}