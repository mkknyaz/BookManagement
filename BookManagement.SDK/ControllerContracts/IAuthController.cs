using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BookManagement.SDK.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace BookManagement.SDK.ControllerContracts
{
    public interface IAuthController
    {
        Task<IActionResult> Registration(RegisterDTO registerDTO);
        Task<IActionResult> Login(LoginDTO loginDTO);
    }
}
