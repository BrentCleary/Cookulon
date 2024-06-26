﻿using Cookulon.Server.Data;
using Cookulon.Server.Services;
using Cookulon.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Cookulon.Server.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class RecipeController : ControllerBase
    {

        private readonly IOpenAIAPI _openAIservice;

        public RecipeController(IOpenAIAPI openAIservice)
        {
            _openAIservice = openAIservice;
        }


        // POST RECIPE IDEAS
        [HttpPost, Route("GetRecipeIdeas")]
        public async Task<ActionResult<List<Idea>>> GetRecipeIdeas(RecipeParms recipeParms)
        {

            string mealTime = recipeParms.MealTime;
            List<string> ingredients = recipeParms.Ingredients
                                                  .Where(i => !string.IsNullOrEmpty(i.Description))
                                                  .Select(i => i.Description!)
                                                  .ToList();

            if (string.IsNullOrEmpty(mealTime))
            {
                mealTime = "Breakfast";
            }

            var ideas = await _openAIservice.CreateRecipeIdeas(mealTime, ingredients);

            return ideas;

            //return SampleData.RecipeIdeas;

        }


        // POST RECIPE
        [HttpPost, Route("GetRecipe")]
        public async Task<ActionResult<Recipe?>> GetRecipe(RecipeParms recipeParms)
        {
            //return SampleData.Recipe;

            List<string> ingredients = recipeParms.Ingredients
                                                  .Where(i => !string.IsNullOrEmpty(i.Description))
                                                  .Select(i => i.Description!)
                                                  .ToList();

            string? title = recipeParms.SelectedIdea;

            if (string.IsNullOrEmpty (title))
            {
                return BadRequest();
            }

            var recipe = await _openAIservice.CreateRecipe(title, ingredients);

            return recipe;
        }


        // GET RECIPE IMAGE
        [HttpGet, Route("GetRecipeImage")]
        public async Task<RecipeImage> GetRecipeImage(string title)
        {
            //return SampleData.RecipeImage;

            var recipeImage = await _openAIservice.CreateRecipeImage(title);

            return recipeImage ?? SampleData.RecipeImage;
            
        }

    }
}
