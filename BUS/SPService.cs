using DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BUS
{
    public class SPService
    {
        private readonly SPModel context = new SPModel();

        public List<SanPham> GetAll()
        {
            return context.SanPham.ToList();
        }

        public string AddProduct(SanPham product)
        {
            try
            {
                if (context.SanPham.Any(p => p.MaSP == product.MaSP))
                {
                    return "Mã sản phẩm đã tồn tại.";
                }
                context.SanPham.Add(product);
                context.SaveChanges();
                return "Thêm sản phẩm thành công!";
            }
            catch (Exception ex)
            {
                return $"Lỗi khi thêm sản phẩm: {ex.Message}";
            }
        }

        public SanPham GetProductById(string productId)
        {
            return context.SanPham.FirstOrDefault(p => p.MaSP == productId);
        }

        public string UpdateProduct(SanPham product)
        {
            try
            {
                var existingProduct = context.SanPham.FirstOrDefault(p => p.MaSP == product.MaSP);
                if (existingProduct == null)
                {
                    return "Sản phẩm không tồn tại.";
                }

                // Update product properties
                existingProduct.TenSP = product.TenSP;
                existingProduct.NgayNhap = product.NgayNhap;
                existingProduct.MaLoai = product.MaLoai;

                context.SaveChanges();
                return "Cập nhật thông tin sản phẩm thành công!";
            }
            catch (Exception ex)
            {
                return $"Lỗi khi cập nhật sản phẩm: {ex.Message}";
            }
        }

        public string DeleteProduct(string productId)
        {
            try
            {
                var existingProduct = context.SanPham.FirstOrDefault(p => p.MaSP == productId);
                if (existingProduct == null)
                {
                    return "Sản phẩm không tồn tại.";
                }

                context.SanPham.Remove(existingProduct);
                context.SaveChanges();
                return "Xóa sản phẩm thành công!";
            }
            catch (Exception ex)
            {
                return $"Lỗi khi xóa sản phẩm: {ex.Message}";
            }
        }
    }
}