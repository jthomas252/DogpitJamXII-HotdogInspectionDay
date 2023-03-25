using Godot;
using System;
using System.Collections.Generic;

public class Hotdog : GrabbableObject
{
    private readonly float SHADER_THRESHOLD_MIN = 0.01f; // Prevent rendering issues with the shader
    private readonly float CHANCE_VALID = 0.6f;
    
    // Frozen stats
    private readonly float FROZEN_CHANCE = 0.9f;
    private readonly float ICE_MAX_SCALE = 1.3f; 
    private readonly float ICE_MIN_SCALE = 0.95f;
    private readonly float FROZEN_TEMPERATURE = 0f;
    private readonly float NORMAL_TEMPERATURE = 10f; 
    private readonly float BURN_TEMPERATURE = 20f;
    
    // Radiation stats
    private readonly float RADIATION_SCALE = 15f;
    
    private readonly float MOLD_DENY_LEVEL = 0.5f;
    private readonly float BURN_DENY_LEVEL = 0.5f;
    private readonly float RAD_DENY_LEVEL = 3.5f; 

    private readonly float MOLD_SHADER_MULT = 1.8f;
    private readonly float BURN_SHADER_MULT = 2.2f; 

    // Child objects
    private Spatial _ice;
    private Label3D _serialNumberLabel;
    private ShaderMaterial _material;
    
    // Internal components
    private HotdogChallenge _challenge;
    private HotdogBrand _brand;
    private List<MeatContent> _meats;
    private string _serialNumber;
    private bool _isValid;
    private float _qualityLevel;
    private float _moldLevel;
    private float _burntLevel;
    private float _radioactivity;
    private float _temperature;
    private string _invalidReason;

    public override void _Ready()
    {
        base._Ready();
        
        // Set up the shader material 
        MeshInstance mesh = GetNode<MeshInstance>("HotdogMesh");
        Material material = mesh.GetSurfaceMaterial(0);
        _material = material as ShaderMaterial;
        _material.ResourceLocalToScene = true;

        // Set relevant stats
        _qualityLevel = GD.Randf();
        _isValid = _qualityLevel >= CHANCE_VALID;
        _temperature = GD.Randf() > FROZEN_CHANCE ? FROZEN_TEMPERATURE : NORMAL_TEMPERATURE;
        _moldLevel = GD.Randf() % (1f - _qualityLevel);
        _burntLevel = 0f;
        _radioactivity = _challenge == HotdogChallenge.RADIOACTIVITY ? GD.Randf() * RADIATION_SCALE : 10f;

        // Update the shader now that we have basic info 
        UpdateShader();

        // Get all child objects that we'll need
        _serialNumberLabel = GetNode<Label3D>("SerialNumber");
        _ice = GetNode<Spatial>("IceMesh");
        _ice.Visible = _temperature < NORMAL_TEMPERATURE;
        
        // Determine which challenge to use, check which are available and fall back if needed
        _challenge = HotdogChallenge.SERIAL_NUMBER;

        // Get the brand to use
        _brand = GetBrand();
        
        // Generate other relevant stats
        _serialNumber = GetSerialNumberFromData();
        _serialNumberLabel.Text = _serialNumber;

        _invalidReason = GetInvalidReasonFromData();
        
        _meats = GetMeatsFromData();
    }

    public string GetInfo()
    {
        string output =
            $"BRAND   {_brand.ToString()}\n" +
            $"SERIAL  {_serialNumber}\n" +
            $"QUALITY {_qualityLevel}\n" +
            $"MOLD    {_moldLevel}\n" +
            $"COLD    {_temperature}\n" +
            $"RADIO   {_radioactivity}\n" +
            $"BURNT   {_burntLevel}\n" + 
            "\n### MEAT CONTENTS ######!\n";
        
        // Iterate through meats and add to output

        return output.ToUpper();
    }

    public override float GetRadiation()
    {
        return _radioactivity; 
    }

    public string GetInvalidReason()
    {
        return _invalidReason;
    }
    
    public bool IsValid()
    {
        if (IsFrozen())
        {
            _invalidReason = "HOTDOG TOO FROZEN";
            return false;
        }
        
        if (_burntLevel > BURN_DENY_LEVEL)
        {
            _invalidReason = "HOTDOG TOO BURNT";
            return false;
        }

        if (_moldLevel > MOLD_DENY_LEVEL)
        {
            _invalidReason = "HOTDOG TOO MOLDY";
            return false;
        }

        if (_radioactivity > RAD_DENY_LEVEL)
        {
            _invalidReason = "HOTDOG TOO RADIOACTIVE";
            return false; 
        }
        
        return _isValid;
    }

    public bool IsFrozen()
    {
        return _temperature < NORMAL_TEMPERATURE; 
    }

    public bool IsBurnt()
    {
        return _burntLevel >= BURN_DENY_LEVEL; 
    }

    public void ApplyHeat(float amount)
    {
        _temperature += amount;
        _ice.Visible = _temperature < NORMAL_TEMPERATURE;
        
        if (_temperature < NORMAL_TEMPERATURE) // Scale ice
        {
            _ice.Scale = Vector3.One * Mathf.Lerp(ICE_MIN_SCALE, ICE_MAX_SCALE, 1f - (_temperature / NORMAL_TEMPERATURE));
        } else if (_temperature > BURN_TEMPERATURE) // Apply burns
        {
            _burntLevel = Mathf.Lerp(0f, 1f, (_temperature - BURN_TEMPERATURE) / BURN_TEMPERATURE);
        }
        UpdateShader();
    }

    private void UpdateShader()
    {
        _material.SetShaderParam("threshold", (_burntLevel > _moldLevel ? _burntLevel : _moldLevel) + SHADER_THRESHOLD_MIN);
        _material.SetShaderParam("burnt", _burntLevel);
        _material.SetShaderParam("mold", _moldLevel);        
    }

    private HotdogBrand GetBrand()
    {
        return _challengeBrands[HotdogChallenge.SERIAL_NUMBER][GD.Randi() % _challengeBrands[HotdogChallenge.SERIAL_NUMBER].Length];
    }

    private string GetInvalidReasonFromData()
    {
        return "INVALID SERIAL NUMBER";
    }

    private string GetSerialNumberFromData()
    {
        if (_isValid)
        {
            return _validSerialNumber[_brand][GD.Randi() % _validSerialNumber[_brand].Length];
        }

        return _invalidSerialNumber[_brand][GD.Randi() % _invalidSerialNumber[_brand].Length];
    }

    // Using the brands and provided lists 
    private List<MeatContent> GetMeatsFromData()
    {
        List<MeatContent> meat = new List<MeatContent>();
        
        // Generate the meats to use
        for (int i = 0; i < 4; ++i)
        {
            
        }
        
        return meat; 
    }

    // ------------
    // Game Data
    // ------------
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
}