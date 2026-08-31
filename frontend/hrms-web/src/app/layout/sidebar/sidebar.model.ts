export interface SidebarItem {
  label: string;
  route: string;
  icon?: string;
  children?: SidebarItem[];
  requiredRoles?: string[];
}
