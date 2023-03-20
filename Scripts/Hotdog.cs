using Godot;
using System;
using System.Collections.Generic;

public class Hotdog : Spatial
{
    private readonly float CHANCE_VALID = 0.5f;

    private bool isValid;

    private string _serialNumber;
    private Label3D _serialNumberLabel;

    // TODO: Nice to have? Move this into JSON or some serialized format rather than in code. 
    public enum HotdogChallenge
    {
        SERIAL_NUMBER,
        MEAT_CONTENT,
        RADIOACTIVITY,
        VISUAL_INSPECTION,
        NOT_HOTDOG
    }

    public enum HotdogBrand
    {
        O_LEARY_GOLDEN,
        BIG_BILL_CHEESE,
        MARTHA_VEGAN,
        WHOLESOME_CHRISTIAN,
        GENERIC
    }

    public enum MeatContent
    {
        // GOOD
        PORK,
        BEEF,
        CHICKEN,
        DONKEY,
        SOY,
        DUCK,
        GOOSE,
        ONION,
        GARLIC,
        CHEESE,

        // OKAY
        RAT,
        WASP,
        BUMBLEBEE,
        PARROT,
        PIGEON,
        HORSE,
        OPOSSUM,
        CRAB,
        FISH,

        // BAD
        GARBAGE,
        ANUS,
        ROACH,
        TEETH,
        HAIR,
        GOLD,
        ROCK,        
        HOPES_DREAMS,
        HUMAN,
        MOLD,
        ASBESTOS,
        ALIEN,
        GREASE,
        FEAR,
        POOP,
        URANIUM,
        UNKNOWN
    }

    private HotdogBrand _brand;
    private List<MeatContent> _meats;

    private Dictionary<HotdogChallenge, HotdogBrand[]> _challengeBrands = new Dictionary<HotdogChallenge, HotdogBrand[]>()
        {
            {
                HotdogChallenge.SERIAL_NUMBER, new HotdogBrand[]
                {
                    HotdogBrand.O_LEARY_GOLDEN,
                    HotdogBrand.BIG_BILL_CHEESE,
                    HotdogBrand.MARTHA_VEGAN,
                    HotdogBrand.WHOLESOME_CHRISTIAN,
                }
            }
        };

    private Dictionary<HotdogBrand, string[]> _validSerialNumber = new Dictionary<HotdogBrand, string[]>()
    {
        {
            HotdogBrand.O_LEARY_GOLDEN, new string[]
            {
                "400683208",
                "422883208",
                "422263208",
                "462663208",
                "442644208",
                "422545208",
                "422540208",
                "400000208",
            }
        },
        {
            HotdogBrand.BIG_BILL_CHEESE, new string[]
            {
                "201250810",
                "203510680",
                "803102525",
                "801010101",
                "203590501",
            }
        },
        {
            HotdogBrand.MARTHA_VEGAN, new string[]
            {
                "107170717",
                "777777777",
                "700067177",
                "717017070",
                "700071717"
            }
        },
        {
            HotdogBrand.WHOLESOME_CHRISTIAN, new string[]
            {
                "126456786",
                "600060006",
                "800666008",
                "100500666",
                "699996996"
            }
        }
    };

    private Dictionary<HotdogBrand, string[]> _invalidSerialNumber = new Dictionary<HotdogBrand, string[]>()
    {
        {
            HotdogBrand.O_LEARY_GOLDEN, new string[]
            {
                "400683209",
                "300683208",
                "412663008",
                "442671208",
                "422945208",
            }
        },
        {
            HotdogBrand.BIG_BILL_CHEESE, new string[]
            {
                "101250810",
                "803102555",
                "205102555",
                "810000000",
            }
        },
        {
            HotdogBrand.MARTHA_VEGAN, new string[]
            {
                "107170711",
                "111111111",
                "170070071",
                "710000017",
            }
        },
        {
            HotdogBrand.WHOLESOME_CHRISTIAN, new string[]
            {
                "420696666",
                "123456789",
                "600000006",
                "106555666",
                "699999996",
                "666666666",
            }
        }
    };

    private Dictionary<HotdogBrand, MeatContent[]> _Meats = new Dictionary<HotdogBrand, MeatContent[]>()
    {
        { HotdogBrand.GENERIC, new MeatContent[]
            {
                MeatContent.PORK,
                MeatContent.BEEF,
                MeatContent.CHICKEN,
            } 
        }
    };

    public override void _Ready()
    {
        _serialNumberLabel = GetNode<Label3D>("GrabbableObject/SerialNumber");
        
        // Set relevant stats
        isValid = (GD.Randf() > CHANCE_VALID);

        // Determine which challenge to use, check which are available and fall back if needed

        // Get the brand to use
        _brand = GetBrand();
        
        // Generate other relevant stats
        _serialNumber = GetSerialNumber();
        _serialNumberLabel.Text = _serialNumber;

        _meats = GetMeats();
    }

    public string GetInfo()
    {
        string output =
            $"BRAND  {_brand.ToString()}\n" +
            $"SERIAL {_serialNumber}\n" +
            $"VALID  {isValid.ToString()}\n" +
            "\n:::MEAT CONTENTS:::\n";
        
        // Iterate through meats and add to output

        return output.ToUpper();
    }

    private HotdogBrand GetBrand()
    {
        return _challengeBrands[HotdogChallenge.SERIAL_NUMBER][GD.Randi() % _challengeBrands[HotdogChallenge.SERIAL_NUMBER].Length];
    }

    private string GetSerialNumber()
    {
        if (isValid)
        {
            return _validSerialNumber[_brand][GD.Randi() % _validSerialNumber[_brand].Length];
        }

        return _invalidSerialNumber[_brand][GD.Randi() % _invalidSerialNumber[_brand].Length];
    }

    // Using the brands and provided lists 
    private List<MeatContent> GetMeats()
    {
        List<MeatContent> meat = new List<MeatContent>();
        
        // Generate the meats to use
        for (int i = 0; i < 4; ++i)
        {
            
        }
        
        return meat; 
    }
}