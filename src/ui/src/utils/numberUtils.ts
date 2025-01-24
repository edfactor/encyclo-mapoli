// src/utils/numberUtils.ts

// This converts numbers to dollars in the summary section
export const currencyFormat =  (num : Number) : string => {
    return '$' + num.toFixed(2).replace(/(\d)(?=(\d{3})+(?!\d))/g, '$1,')
}