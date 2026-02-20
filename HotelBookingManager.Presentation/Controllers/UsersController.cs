using HotelBookingManager.BusinessObjects.DTO;
using HotelBookingManager.BusinessObjects.IService;
using HotelBookingManager.Presentation.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;

namespace HotelBookingManager.Presentation.Controllers
{
    public class UsersController : Controller
    {
        private readonly IUserService _userService;
        private readonly IHotelService _hotelService;

        public UsersController(IUserService userService, IHotelService hotelService)
        {
            _userService = userService;
            _hotelService = hotelService;
        }

        private bool IsAdmin()
        {
            var roleClaim = User.FindFirstValue(ClaimTypes.Role);
            return !string.IsNullOrEmpty(roleClaim) && roleClaim == "1";
        }

        private static UserManageViewModel ToViewModel(UserDto dto)
        {
            return new UserManageViewModel
            {
                UserId = dto.UserId,
                FullName = dto.FullName,
                Email = dto.Email,
                PhoneNumber = dto.PhoneNumber,
                RoleId = dto.RoleId,
                HotelId = dto.HotelId ?? 0,
                HotelName = dto.HotelName,
                IsActive = dto.IsActive
            };
        }

        private static UserDto ToDto(UserManageViewModel vm)
        {
            return new UserDto
            {
                UserId = vm.UserId,
                FullName = vm.FullName,
                Email = vm.Email,
                PhoneNumber = vm.PhoneNumber,
                RoleId = vm.RoleId,
                HotelId = vm.HotelId,
                IsActive = vm.IsActive
            };
        }

        // GET: Users/Index
        public async Task<IActionResult> Index()
        {
            if (!IsAdmin()) return Forbid();
            var dtos = await _userService.GetAllAsync();
            var vms = dtos.Select(ToViewModel).ToList();
            return View(vms);
        }


        // GET: Users/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            if (!IsAdmin()) return Forbid();

            var dto = await _userService.GetByIdAsync(id);
            if (dto == null) return NotFound();

            var vm = ToViewModel(dto);

            // ✅ ROLES - Luôn safe
            var roles = new List<SelectListItem>
    {
        new SelectListItem { Value = "1", Text = "Admin" },
        new SelectListItem { Value = "2", Text = "Staff" },
        new SelectListItem { Value = "3", Text = "Customer" }
    };
            ViewBag.Roles = new SelectList(roles, "Value", "Text", vm.RoleId.ToString());

            // ✅ HOTELS - Safe handling
            List<SelectListItem> hotelItems = new List<SelectListItem>
    {
        new SelectListItem { Value = "", Text = "-- Chọn Hotel --" }
    };

            try
            {
                var hotels = await _hotelService.GetAllAsync();
                if (hotels != null)
                {
                    foreach (var hotel in hotels)
                    {
                        hotelItems.Add(new SelectListItem
                        {
                            Value = hotel.HotelId.ToString(),
                            Text = hotel.Name ?? "Không tên"
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                // Log error nếu cần
                Console.WriteLine($"HotelService error: {ex.Message}");
            }

            ViewBag.Hotels = new SelectList(hotelItems, "Value", "Text", vm.HotelId?.ToString() ?? "");

            return View(vm);
        }


        // POST: Users/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, UserManageViewModel model)
        {
            if (!IsAdmin()) return Forbid();
            if (id != model.UserId) return BadRequest();

            if (!ModelState.IsValid)
            {
                // ✅ RELOAD DROPDOWNS - SAME LOGIC
                var roles = new List<SelectListItem>
        {
            new SelectListItem { Value = "1", Text = "Admin" },
            new SelectListItem { Value = "2", Text = "Staff" },
            new SelectListItem { Value = "3", Text = "Customer" }
        };
                ViewBag.Roles = new SelectList(roles, "Value", "Text", model.RoleId.ToString());

                List<SelectListItem> hotelItems = new List<SelectListItem>
        {
            new SelectListItem { Value = "", Text = "-- Chọn Hotel --" }
        };

                try
                {
                    var hotels = await _hotelService.GetAllAsync();
                    if (hotels != null)
                    {
                        foreach (var hotel in hotels)
                        {
                            hotelItems.Add(new SelectListItem
                            {
                                Value = hotel.HotelId.ToString(),
                                Text = hotel.Name ?? "Không tên"
                            });
                        }
                    }
                }
                catch { /* Ignore */ }

                ViewBag.Hotels = new SelectList(hotelItems, "Value", "Text", model.HotelId?.ToString() ?? "");
                return View(model);
            }

            var dto = ToDto(model);
            var ok = await _userService.UpdateAsync(dto);
            if (!ok)
            {
                ModelState.AddModelError("", "Cập nhật thất bại!");
                // Reload dropdowns...
                return View(model);
            }

            return RedirectToAction(nameof(Index));
        }


        // GET: Users/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            if (!IsAdmin()) return Forbid();

            var dto = await _userService.GetByIdAsync(id);
            if (dto == null) return NotFound();

            var vm = ToViewModel(dto);
            return View(vm);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (!IsAdmin()) return Forbid();

            await _userService.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }

        private async Task PopulateDropdowns(UserManageViewModel model)
        {
            // Roles
            ViewBag.Roles = new SelectList(new[]
            {
                new { Id = 1, Name = "Admin" },
                new { Id = 2, Name = "Staff" },
                new { Id = 3, Name = "Customer" }
            }, "Id", "Name", model.RoleId);

            // Hotels
            try
            {
                var hotels = await _hotelService.GetAllAsync() ?? new List<HotelDto>();
                ViewBag.Hotels = new SelectList(hotels, "Id", "Name", model.HotelId);
            }
            catch
            {
                ViewBag.Hotels = new SelectList(new List<object>(), "Id", "Name");
            }
        }
    }
}
