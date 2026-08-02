export class ShopParams {
    brands: string[] = [];
    types: string[] = [];
    sort: string = 'name'; // Default sort option
    pageIndex: number = 1;
    pageSize: number = 5;
    search: string = '';
}