using System.Collections.Generic;
using System.Linq;
using introduccion.Data;

namespace introduccion.Database;

public class ServicioBaseDatos
{
    private readonly ContextoDatos _contextoDatos;

    public ServicioBaseDatos(ContextoDatos contextoDatos)
    {
        _contextoDatos = contextoDatos;
        _contextoDatos.Database.EnsureCreated();
    }

    public void Insertar<T>(T entidad) where T : class
    {
        _contextoDatos.Set<T>().Add(entidad);
    }

    public void Eliminar<T>(T entidad) where T : class
    {
        _contextoDatos.Set<T>().Remove(entidad);
    }

    public List<T> LeerTodos<T>() where T : class
    {
        return _contextoDatos.Set<T>().ToList();
    }

    public IQueryable<T> Tabla<T>() where T : class
    {
        return _contextoDatos.Set<T>();
    }

    public bool Existe<T>(System.Func<T, bool> filtro) where T : class
    {
        return _contextoDatos.Set<T>().Any(filtro);
    }

    public void GuardarCambios()
    {
        _contextoDatos.SaveChanges();
    }
}