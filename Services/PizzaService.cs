using aspnetcore_with_reactspa.Models;
using aspnetcore_with_reactspa.Data;
using Microsoft.EntityFrameworkCore;

namespace aspnetcore_with_reactspa.Services;

public class PizzaService
{
    private readonly PizzaContext _context;

    public PizzaService(PizzaContext context)
    {
        _context = context;
    }

    public IEnumerable<Topping> ToppingGetAll()
    {
       return _context.Toppings
        .AsNoTracking()
        .ToList();

    }

    public IEnumerable<Sauce> SauceGetAll()
    {
       return _context.Sauces
        .AsNoTracking()
        .ToList();

    }

    public IEnumerable<Pizza> GetAll()
    {
       return _context.Pizzas
        .AsNoTracking()
        .ToList();

    }



    public Pizza? GetById(int id)
    {
         return _context.Pizzas
            .Include(p => p.Toppings)
            .Include(p => p.Sauce)
            //.AsNoTracking()
            .SingleOrDefault(p => p.Id == id);

    }

    public Pizza? Create(Pizza newPizza)
    {
        
        var pizza = new Pizza  {
            Id=newPizza.Id,
            Name=newPizza.Name
        };

        _context.Pizzas.Add(pizza);
        _context.SaveChanges();


        UpdateSauce(pizza.Id, newPizza.Sauce.Id);

        foreach (var item in newPizza.Toppings)
        {
            AddTopping(pizza.Id, item.Id);
        }


        return newPizza;
    }
    public Pizza? updatePizza(Pizza PizzaUp){
        var pizza = _context.Pizzas
        .Include(p => p.Toppings)
        .Include(p => p.Sauce)
        .SingleOrDefault(p => p.Id == PizzaUp.Id);

        foreach(Topping t in pizza.Toppings.ToList()){
            pizza.Toppings.Remove(t);
        }
        _context.Pizzas.Update(pizza);
        _context.SaveChanges();
        
        PizzaUp.Toppings.ToList().ForEach(
            Item => AddTopping(pizza.Id, Item.Id) 
        );
        _context.Pizzas.Update(pizza);
        _context.SaveChanges();
        if(PizzaUp.Name != pizza.Name ){
            pizza.Name = PizzaUp.Name;
            _context.Pizzas.Update(pizza);
            _context.SaveChanges();
        }


        return pizza;
    }

    public void AddTopping(int PizzaId, int ToppingId)
    {
        var pizzaToUpdate = _context.Pizzas.Find(PizzaId);
        var toppingToAdd = _context.Toppings.Find(ToppingId);

        if (pizzaToUpdate is null || toppingToAdd is null)
        {
            throw new NullReferenceException("Pizza or topping does not exist");
        }

        if(pizzaToUpdate.Toppings is null)
        {
            pizzaToUpdate.Toppings = new List<Topping>();
        }

        pizzaToUpdate.Toppings.Add(toppingToAdd);

        _context.Pizzas.Update(pizzaToUpdate);
        _context.SaveChanges();

    }

    public void UpdateSauce(int PizzaId, int SauceId)
    {
            var pizzaToUpdate = _context.Pizzas.Find(PizzaId);
            var sauceToUpdate = _context.Sauces.Find(SauceId);

            if (pizzaToUpdate is null || sauceToUpdate is null)
            {
                throw new NullReferenceException("Pizza or sauce         does not exist");
            }

            pizzaToUpdate.Sauce = sauceToUpdate;

            _context.SaveChanges();

    }

    public void DeleteById(int id)
    {
         var pizzaDel = _context.Pizzas.Find(id);
        if(pizzaDel is null){
            throw new NullReferenceException("Pizza does not exist");       
        }
        _context.Pizzas.Remove(pizzaDel);
        _context.SaveChanges();
    }
}