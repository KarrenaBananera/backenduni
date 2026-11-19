export class Product {
  constructor(
    public id: number,
    public name: string,
    public imageUrl: string,
    public images: string[],         
    public characteristics: Record<string, string>,
    public price: number,
    public rating: number,
    public description: string,
    public category: string
  ) {}
}