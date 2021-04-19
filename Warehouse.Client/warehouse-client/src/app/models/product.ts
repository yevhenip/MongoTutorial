import {Customer} from "./customer";
import {Manufacturer} from "./manufacturer";

export interface Product {
  id: string,
  name: string,
  dateOfReceipt: Date,
  manufacturers: Manufacturer[],
  customer: Customer
}
