using EasyGamesWeb.Constants;
using EasyGamesWeb.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace EasyGamesWeb.Controllers
{
    [Authorize(Roles = nameof(Roles.Admin))]
    public class UsersController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UsersController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<IActionResult> Index()
        {
            var users = _userManager.Users.ToList();
            var rows = new List<UserRowVM>();

            foreach (var u in users)
            {
                var roles = await _userManager.GetRolesAsync(u);
                rows.Add(new UserRowVM
                {
                    Id = u.Id,
                    Email = u.Email ?? "",
                    Roles = roles
                });
            }

            return View(rows);
        }

        public IActionResult Create()
        {
            ViewBag.Roles = GetRoleSelectList();
            return View(new UserCreateVM());
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UserCreateVM vm)
        {
            ViewBag.Roles = GetRoleSelectList();
            if (!ModelState.IsValid) return View(vm);

            var exists = await _userManager.FindByEmailAsync(vm.Email);
            if (exists != null)
            {
                ModelState.AddModelError(nameof(vm.Email), "Email already exists.");
                return View(vm);
            }

            var user = new IdentityUser
            {
                UserName = vm.Email,
                Email = vm.Email,
                EmailConfirmed = true
            };

            var createResult = await _userManager.CreateAsync(user, vm.Password);
            if (!createResult.Succeeded)
            {
                foreach (var e in createResult.Errors) ModelState.AddModelError("", e.Description);
                return View(vm);
            }

            if (!await _roleManager.RoleExistsAsync(vm.Role))
                await _roleManager.CreateAsync(new IdentityRole(vm.Role));

            await _userManager.AddToRoleAsync(user, vm.Role);

            TempData["SuccessMsg"] = "User created.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            var roles = await _userManager.GetRolesAsync(user);
            var vm = new UserEditVM
            {
                Id = user.Id,
                Email = user.Email ?? "",
                Role = roles.FirstOrDefault() ?? Roles.User.ToString()
            };

            ViewBag.Roles = GetRoleSelectList();
            return View(vm);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UserEditVM vm)
        {
            ViewBag.Roles = GetRoleSelectList();
            if (!ModelState.IsValid) return View(vm);

            var user = await _userManager.FindByIdAsync(vm.Id);
            if (user == null) return NotFound();

            var currentUserId = _userManager.GetUserId(User);
            if (user.Id == currentUserId && vm.Role != Roles.Admin.ToString())
            {
                ModelState.AddModelError("", "You cannot remove your own Admin role.");
                return View(vm);
            }

            user.Email = vm.Email;
            user.UserName = vm.Email;

            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                foreach (var e in updateResult.Errors) ModelState.AddModelError("", e.Description);
                return View(vm);
            }

            var currentRoles = await _userManager.GetRolesAsync(user);
            if (!currentRoles.Contains(vm.Role) || currentRoles.Count != 1)
            {
                await _userManager.RemoveFromRolesAsync(user, currentRoles);
                if (!await _roleManager.RoleExistsAsync(vm.Role))
                    await _roleManager.CreateAsync(new IdentityRole(vm.Role));
                await _userManager.AddToRoleAsync(user, vm.Role);
            }

            TempData["SuccessMsg"] = "User updated.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> ResetPassword(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            var vm = new ResetPasswordVM { Id = user.Id, Email = user.Email ?? "" };
            return View(vm);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordVM vm)
        {
            if (!ModelState.IsValid) return View(vm);

            var user = await _userManager.FindByIdAsync(vm.Id);
            if (user == null) return NotFound();

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, token, vm.NewPassword);

            if (!result.Succeeded)
            {
                foreach (var e in result.Errors) ModelState.AddModelError("", e.Description);
                return View(vm);
            }

            TempData["SuccessMsg"] = "Password reset.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            if (_userManager.GetUserId(User) == user.Id)
            {
                TempData["ErrorMsg"] = "You cannot delete your own account.";
                return RedirectToAction(nameof(Index));
            }

            return View(user);
        }

        [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            if (_userManager.GetUserId(User) == user.Id)
            {
                TempData["ErrorMsg"] = "You cannot delete your own account.";
                return RedirectToAction(nameof(Index));
            }

            var roles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, roles);

            var result = await _userManager.DeleteAsync(user);
            TempData[result.Succeeded ? "SuccessMsg" : "ErrorMsg"] =
                result.Succeeded ? "User deleted." : string.Join(" | ", result.Errors.Select(e => e.Description));

            return RedirectToAction(nameof(Index));
        }

        private List<string> GetRoleSelectList() => Enum.GetNames(typeof(Roles)).ToList();
    }
}
