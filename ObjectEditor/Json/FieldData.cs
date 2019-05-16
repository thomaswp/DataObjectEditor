namespace ObjectEditor.Json
{

    using System;
    using System.Collections.Generic;

    public interface FieldData
    {

        bool writeMode();

        bool readMode();

        int add(int x, String field);

        long add(long x, String field);

        short add(short x, String field);

        float add(float x, String field);

        double add(double x, String field);

        byte add(byte x, String field);

        char add(char x, String field);

        bool add(bool x, String field);

        String add(String x, String field);

        Reference<T> addReference<T>(Reference<T> x, String field)  where T : GuidDataObject;

        T addReference<T>(T x, String field) where T : GuidDataObject;

        Guid addGuid(Guid x, String field);

        T addEnum<T>(T x, String field) where T : struct;

        T addEnum<T>(T x, T deflt, String field) where T : struct;

        T add<T>(T x, String field) where T : DataObject;

        T[] addPrimitiveArray<T>(T[] x, String field) where T : struct;

        List<T> addList<T>(List<T> x, String field) where T : DataObject;

        List<T> addList<T>(List<T> x, String field, Func<T, T> f);

        List<T> addPrimitiveList<T>(List<T> x, String field) where T : struct;

        Dictionary<T, U> addDictionary<T, U>(Dictionary<T, U> x, String field, Func<T, T> fT, Func<U, U> fU);
    }
}