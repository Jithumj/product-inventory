export interface VariantOption {
  variant: string
  option: string
}

export interface VariantCombination {
  id: string
  combinationCode: string
  stock: number
  options: VariantOption[]
}

export interface Variant {
  name: string
  options: string[]
}

export interface Product {
  id: string
  productCode: string
  productName: string
  variants: Variant[]
  variantCombinations: VariantCombination[]
  // Optionally include legacy fields for compatibility
  productImage?: string | null
  createdDate?: string
  updatedDate?: string
  createdUser?: string
  isFavourite?: boolean
  active?: boolean
  hsnCode?: string
  totalStock?: number
} 