using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TopsyTurvyCakes.Models;

namespace TopsyTurvyCakes.Pages.Admin
{
    [Authorize]
    public class AddEditRecipeModel : PageModel
    {
        private readonly IRecipesService _IR;
        public AddEditRecipeModel(IRecipesService IR)
        {
            _IR = IR;
        }
        [FromRoute]
        [Required]
        public long? Id { get; set; }

        [Required]
        public bool IsNewRecipe {get {  return Id == null;}}
       
        [BindProperty]
        public Recipe Recipe { get; set; }
     
        [BindProperty]
        public IFormFile Image { get; set; }

        //HERE WHEN FIRST THE PAGE LOADS THIS GET METHOD RUNS 
        //HERE WHEN FIRST THE PAGE LOADS THIS GET METHOD RUNS 
        public async Task OnGetAsync()
        {
            //FIND THAT ID AND POPULATE THE FORM WITH IT,
            //GETVALUEORDEFAUL => CHANGES NULLABLE LONG VALUE TO REGULAR LONG VALUE / IF ID IS NULL THNE CREATE RECIPE
            Recipe = await _IR.FindAsync(Id.GetValueOrDefault()) 
                ?? new Recipe();
        }

        public async Task<IActionResult> OnPostAsync()
        {

            if (!ModelState.IsValid)
            {
                return Page();
            }
         var   recipe = await _IR.FindAsync(Id.GetValueOrDefault())
             ?? new Recipe();

            recipe.Name = Recipe.Name;
            recipe.Description = Recipe.Description;
            recipe.Ingredients = Recipe.Ingredients;
            recipe.Directions = Recipe.Directions;

            if (Image != null) {
                using (var stream = new System.IO.MemoryStream())
                {
                    await Image.CopyToAsync(stream);
                    recipe.Image = stream.ToArray();
                    recipe.ImageContentType = Image.ContentType;
                }
               
            } 
          
            await _IR.SaveAsync(recipe);
            return RedirectToPage("/Recipe", new { id = recipe.Id }) ;
        }

        public async Task<IActionResult> OnPostDelete()
        {

            _IR.DeleteAsync(Id.Value);
            return RedirectToPage("/Index");

        }
    }
}
