using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ObjectPool<T>
{
    public delegate T FactoryMethod();
    FactoryMethod _factory;

    Action<T> _turnOff;
    Action<T> _turnOn;

    List<T> _stock = new List<T>();

    public ObjectPool(FactoryMethod factory, Action<T> TurnOff, Action<T> TurnOn, int initialCount = 5)
    {
        _factory = factory;
        _turnOff = TurnOff;
        _turnOn = TurnOn;

        for (int i = 0; i < initialCount; i++)
        {
            var obj = _factory();
            _turnOff(obj);
            _stock.Add(obj);
        }
    }

    public T Get()
    {
        T obj = default;

        if (_stock.Count > 0)
        {
            obj = _stock[0];
            _stock.RemoveAt(0);
        }
        else
        {
            obj = _factory();
        }

        _turnOn(obj);
        return obj;
    }

    public void StockAdd(T obj)
    {
        _turnOff(obj);
        _stock.Add(obj);
    }

}
