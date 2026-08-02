import { Component, inject, OnInit, signal } from '@angular/core';
import { ShopService } from '../../core/services/shop.service';
import { Product } from '../../shared/models/product';
import { ProductItemComponent } from "./product-item/product-item.component";
import { MatDialog } from '@angular/material/dialog';
import { FiltersDialogComponent } from './filters-dialog/filters-dialog.component';
import { MatButton } from '@angular/material/button';
import { MatIcon } from "@angular/material/icon";
import { Observable, switchMap, tap } from 'rxjs';
import { MatListOption, MatSelectionList, MatSelectionListChange } from '@angular/material/list';
import { MatMenu, MatMenuTrigger } from '@angular/material/menu';
import { Pagination } from '../../shared/models/pagination';
import { ShopParams } from '../../shared/models/shop.params';
import { MatPaginator, PageEvent } from '@angular/material/paginator';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-shop',
  imports: [
    ProductItemComponent,
    MatButton,
    MatIcon,
    MatMenu,
    MatSelectionList,
    MatListOption,
    MatMenuTrigger,
    MatPaginator,
    FormsModule
],
  templateUrl: './shop.component.html',
  styleUrl: './shop.component.scss',
})
export class ShopComponent implements OnInit {
  protected shopService = inject(ShopService);
  protected dialogService = inject(MatDialog);
  products = signal<Pagination<Product> | undefined>(undefined);

  selectedBrands: string[] = [];
  selectedTypes: string[] = [];

  selectedSort: string = 'name'; // Default sort option
  sortOptions: { name: string, value: string }[] = [
    { name: 'Alphabetical', value: 'name' },
    { name: 'Price: Low-High', value: 'priceAsc' },
    { name: 'Price: High-Low', value: 'priceDesc' }
  ];
  shopParams = new ShopParams();
  pageSizeOptions = [5, 10, 15];

  ngOnInit() {  
    this.initializeShop();
    this.getProducts().subscribe({
      next: response => this.products.set(response),
      error: error => console.error('Error fetching products:', error)
    });;
  }

  initializeShop() {
    this.shopService.getBrands();
    this.shopService.getTypes();
  }
  
  getProducts(): Observable<Pagination<Product>> {
    return this.shopService.getProducts(this.shopParams);
  }

  onSearchChange() {
    this.shopParams.pageIndex = 1;
    this.getProducts().subscribe({
      next: response => this.products.set(response),
      error: error => console.error('Error fetching products:', error)
    });
  }

  handlePageEvent(event: PageEvent) {
    this.shopParams.pageIndex = event.pageIndex + 1; 
    this.shopParams.pageSize = event.pageSize;
    this.getProducts().subscribe({
      next: response => this.products.set(response),
      error: error => console.error('Error fetching products:', error)
    });
  }

  openFiltersDialog() {
    const dialogRef = this.dialogService.open(FiltersDialogComponent, {
      minWidth: '500px',
      data: {
        selectedBrands: this.shopParams.brands,
        selectedTypes: this.shopParams.types
      }
    });

    dialogRef.afterClosed().pipe(
      tap(res => {
        if (res?.selectedBrands) this.shopParams.brands = res.selectedBrands;
        if (res?.selectedTypes) this.shopParams.types = res.selectedTypes;
        this.shopParams.pageIndex = 1;
      }),
      switchMap(() => this.getProducts())
    ).subscribe({
      next: response => {
        this.products.set(response);
      },
      // error: error => console.error('Error fetching products:', error)
    });
  }

  onSortChange(event: MatSelectionListChange) {
    const selectedOption = event.options[0];
    if (selectedOption) {
      this.shopParams.sort = selectedOption.value;
    }
    this.getProducts().subscribe({
      next: response => {
        this.products.set(response);
        this.shopParams.pageIndex = 1;
      },
      error: error => console.error('Error fetching products:', error)
    });
  }
}
