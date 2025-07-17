export interface AddProductVariant {
  name: string
  options: string[]
}

export interface AddProduct {
  productCode: string
  productName: string
  createdUser: string
  variants: AddProductVariant[]
} 