import { HttpClient } from '@angular/common/http';
import { Component, inject, OnInit } from '@angular/core';
import { ShopService } from '../../../core/services/shop.service';

@Component({
  selector: 'app-deneme',
  standalone: true,
  imports: [],
  templateUrl: './deneme.component.html',
  styleUrl: './deneme.component.scss'
})
export class DenemeComponent implements OnInit {
  baseUrl = 'https://localhost:5001/api/'

  types: string[] = [];
  brands: string[] = [];

  private http = inject(HttpClient)

  private shopService = inject(ShopService)

  ngOnInit(): void {
    this.getBrands();

    this.shopService.getTypesTest()?.subscribe({
      next: response => this.types = response
    })
  }

  getBrands() {
    if (this.brands.length > 0) return;
    return this.http.get<string[]>(this.baseUrl + 'products/brands').subscribe({
      next: response => this.brands = response,
    })
  }

  getTypes() {
    if (this.types.length > 0) return;
    return this.http.get<string[]>(this.baseUrl + 'products/types').subscribe({
      next: response => this.types = response,
    })
  }
}
