using Godot;
using System;
using System.Collections.Generic;

public class Hotdog : GrabbableObject
{
    [Export] public AudioStream[] hotdogNoises;

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
    private AudioStreamPlayer3D _audioPlayer;

    // Internal components
    private HotdogChallenge _challenge;
    private HotdogBrand _brand;
    private List<string> _meats;
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

        // Determine which challenge to use, check which are available and fall back if needed
        _challenge = GetChallenge();

        // Set relevant stats
        _qualityLevel = GD.Randf();
        _isValid = _qualityLevel >= CHANCE_VALID;
        _temperature = GD.Randf() > FROZEN_CHANCE && BaseScene.GetLevel() >= 2
            ? FROZEN_TEMPERATURE
            : NORMAL_TEMPERATURE;
        _moldLevel = _challenge == HotdogChallenge.VISUAL_INSPECTION ? GD.Randf() % (1f - _qualityLevel) : 0f;
        _burntLevel = _challenge == HotdogChallenge.VISUAL_INSPECTION ? GD.Randf() % (1f - _qualityLevel) : 0f;
        _radioactivity = _challenge == HotdogChallenge.RADIOACTIVITY ? GD.Randf() * RADIATION_SCALE : 0f;

        // Update the shader now that we have basic info 
        UpdateShader();

        // Get all child objects that we'll need
        _audioPlayer = GetNode<AudioStreamPlayer3D>("Sound");
        _serialNumberLabel = GetNode<Label3D>("SerialNumber");
        _ice = GetNode<Spatial>("IceMesh");
        _ice.Visible = _temperature < NORMAL_TEMPERATURE;

        // Get the brand to use
        _brand = GetBrand();

        // Generate other relevant stats
        _serialNumber = GetSerialNumberFromData();
        _serialNumberLabel.Text = _serialNumber;

        _invalidReason = GetInvalidReasonFromData();

        _meats = GetMeatsFromData();

        Connect("body_entered", this, nameof(OnCollision));
    }

    public void OnCollision(Node node)
    {
        float velocity = Vector3.Zero.DistanceTo(LinearVelocity);
        _audioPlayer.Stream = GetHotdogNoise();
        _audioPlayer.Play();
    }

    private AudioStream GetHotdogNoise()
    {
        return hotdogNoises[GD.Randi() % hotdogNoises.Length];
    }

    private HotdogChallenge GetChallenge()
    {
        HotdogChallenge[] challenges = BaseScene.GetLevel() >= 2
            ? new HotdogChallenge[]
            {
                HotdogChallenge.SERIAL_NUMBER,
                HotdogChallenge.VISUAL_INSPECTION,
                HotdogChallenge.MEAT_CONTENT
            }
            : new HotdogChallenge[]
            {
                HotdogChallenge.SERIAL_NUMBER,
                HotdogChallenge.VISUAL_INSPECTION,
                HotdogChallenge.MEAT_CONTENT,
                HotdogChallenge.RADIOACTIVITY
            };

        return challenges[GD.Randi() % challenges.Length];
    }

    public string GetInfo()
    {
        string output =
            $"BRAND   {_brand.ToString()}\n" +
            $"SERIAL  {_serialNumber}\n" +
            "\n### MEAT CONTENTS #############################\n";

        // Iterate through meats and add to output
        foreach (string meat in _meats)
        {
            output += $"{meat}\n";
        }

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
            _ice.Scale = Vector3.One *
                         Mathf.Lerp(ICE_MIN_SCALE, ICE_MAX_SCALE, 1f - (_temperature / NORMAL_TEMPERATURE));
        }
        else if (_temperature > BURN_TEMPERATURE) // Apply burns
        {
            _burntLevel = Mathf.Lerp(0f, 1f, (_temperature - BURN_TEMPERATURE) / BURN_TEMPERATURE);
        }

        UpdateShader();
    }

    private void UpdateShader()
    {
        _material.SetShaderParam("threshold",
            (_burntLevel > _moldLevel ? _burntLevel : _moldLevel) + SHADER_THRESHOLD_MIN);
        _material.SetShaderParam("burnt", _burntLevel);
        _material.SetShaderParam("mold", _moldLevel);
    }

    private HotdogBrand GetBrand()
    {
        return _challengeBrands[_challenge][GD.Randi() % _challengeBrands[_challenge].Length];
    }

    private string GetInvalidReasonFromData()
    {
        switch (_challenge)
        {
            case HotdogChallenge.SERIAL_NUMBER:
                return "INVALID SERIAL NUMBER";
            
            case HotdogChallenge.MEAT_CONTENT:
                return "UNACCEPTABLE MEATS";
            
            case HotdogChallenge.RADIOACTIVITY:
                return "TOO RADIOACTIVE";
            
            case HotdogChallenge.VISUAL_INSPECTION:
                return "FAILED VISUAL INSPECTION"; 
            
            default:
                return "FUCK YOU"; 
        }
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
    private List<string> GetMeatsFromData()
    {
        List<string> contents = new List<string>();
        string[] order = new string[] {"good", "questionable", "bad", "bad"}; // Default for invalid
        
        // Do specific brand or challenge meats here

        // Determine good vs. bad to use 
        if (_isValid)
        {
            order = _challenge == HotdogChallenge.MEAT_CONTENT ? 
                new string[] { "good", "good", "questionable", "questionable" } :
                new string[] { "good", "good", "good", "questionable" };
        } else if (_challenge == HotdogChallenge.MEAT_CONTENT)
        {
            order = new string[] { "bad", "questionable", "bad", "questionable" };
        }
        
        // Generate the meats to use
        for (int i = contents.Count; i < 4; ++i)
        {
            contents.Add(MeatContent[order[i]][GD.Randi() % MeatContent[order[i]].Length]);
        }

        return contents;
    }

    // ------------
    // Game Data
    // ------------
    public enum HotdogChallenge
    {
        SERIAL_NUMBER,
        MEAT_CONTENT,
        RADIOACTIVITY,
        VISUAL_INSPECTION
    }

    public enum HotdogBrand
    {
        O_LEARY_GOLDEN,
        BIG_BILL_CHEESE,
        MARTHA_VEGAN,
        WHOLESOME_CHRISTIAN,
        CHERNOBYTES,
        GENERIC
    }

    public Dictionary<string, string[]> MeatContent = new Dictionary<string, string[]>()
    {
        {
            // GOOD
            "good", new string[]
                { 
                "PORK", "BEEF", "CHICKEN", "SOY", "DUCK", "GOOSE", "ONION", "GARLIC", "CHEESE", 
                "CRAB", "FISH", "WALRUS", "OCTOPUS", "YAK", "ALLIGATOR", "SNAKE", "ELEPHANT",
                "LAMB", "GOAT", "BOAR", "GIRAFFE", "HIPPO", "LOBSTER"
                }
        },
        {
            // QUESTIONABLE
            "questionable", new string[]
            {
                "RAT", "BABA", "WASP", "BUMBLEBEE", "PIGEON", "OPOSSUM", "RACCOON", "HORSE", "PARROT", "DONKEY",
                "\"BEEF\"", "BEANS", "CORN", "KORN", "MILK", "CHILI", "PANDA", "GRIZZLY_BEAR", "HOTDOG?", "MONKEY",
                "SPIDERS"
            }
        },        
        {
            // BAD
            "bad", new string[]
            {
                "GARBAGE", "ANUSES", "ROACHES", "TEETH", "HAIR", "ROCKS", "HOPES_DREAMS", "HUMAN",
                "ASBESTOS", "ALIEN", "GREASE", "FEAR", "POOP", "URANIUM", "UNKNOWN", "FEAR", "ANGER", "LOVE",
                "WRATH", "ENVY", "ANTS", "SASQUATCH", "YETI", "LANGOLIERS", "MATH", "QUATERNIONS", "WOOD",
                "POISON", "EYEBALLS", "BRAINS", "CRAYONS"
            }
        }
    };


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
            },
            {
                HotdogChallenge.VISUAL_INSPECTION, new HotdogBrand[]
                {
                    HotdogBrand.O_LEARY_GOLDEN,
                    HotdogBrand.BIG_BILL_CHEESE,
                    HotdogBrand.MARTHA_VEGAN,
                    HotdogBrand.WHOLESOME_CHRISTIAN,
                }
            },     
            {
                HotdogChallenge.MEAT_CONTENT, new HotdogBrand[]
                {
                    HotdogBrand.O_LEARY_GOLDEN,
                    HotdogBrand.BIG_BILL_CHEESE,
                    HotdogBrand.MARTHA_VEGAN,
                    HotdogBrand.WHOLESOME_CHRISTIAN,
                }
            },              
            {
                HotdogChallenge.RADIOACTIVITY, new HotdogBrand[]
                {
                    HotdogBrand.O_LEARY_GOLDEN,
                    HotdogBrand.BIG_BILL_CHEESE,
                    HotdogBrand.MARTHA_VEGAN,
                    HotdogBrand.WHOLESOME_CHRISTIAN,
                }
            },            
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
}