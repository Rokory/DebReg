using DebReg.Models;
using DebReg.Security;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;

namespace DebReg.Data.Migrations
{

	public sealed class Configuration : DbMigrationsConfiguration<DebReg.Data.DebRegContext>
	{
		#region Countries
		private Country[] countries = new Country[] {
			new Country {ShortName = "Afghanistan", Alpha2 = "AF", Alpha3 = "AFG", NumericCode = 4},
			new Country {ShortName = "Åland Islands", Alpha2 = "AX", Alpha3 = "ALA", NumericCode = 248},
			new Country {ShortName = "Albania", Alpha2 = "AL", Alpha3 = "ALB", NumericCode = 8},
			new Country {ShortName = "Algeria", Alpha2 = "DZ", Alpha3 = "DZA", NumericCode = 12},
			new Country {ShortName = "American Samoa", Alpha2 = "AS", Alpha3 = "ASM", NumericCode = 16},
			new Country {ShortName = "Andorra", Alpha2 = "AD", Alpha3 = "AND", NumericCode = 20},
			new Country {ShortName = "Angola", Alpha2 = "AO", Alpha3 = "AGO", NumericCode = 24},
			new Country {ShortName = "Anguilla", Alpha2 = "AI", Alpha3 = "AIA", NumericCode = 660},
			new Country {ShortName = "Antarctica", Alpha2 = "AQ", Alpha3 = "ATA", NumericCode = 10},
			new Country {ShortName = "Antigua and Barbuda", Alpha2 = "AG", Alpha3 = "ATG", NumericCode = 28},
			new Country {ShortName = "Argentina", Alpha2 = "AR", Alpha3 = "ARG", NumericCode = 32},
			new Country {ShortName = "Armenia", Alpha2 = "AM", Alpha3 = "ARM", NumericCode = 51},
			new Country {ShortName = "Aruba", Alpha2 = "AW", Alpha3 = "ABW", NumericCode = 533},
			new Country {ShortName = "Australia", Alpha2 = "AU", Alpha3 = "AUS", NumericCode = 36},
			new Country {ShortName = "Austria", Alpha2 = "AT", Alpha3 = "AUT", NumericCode = 40},
			new Country {ShortName = "Azerbaijan", Alpha2 = "AZ", Alpha3 = "AZE", NumericCode = 31},
			new Country {ShortName = "Bahamas", Alpha2 = "BS", Alpha3 = "BHS", NumericCode = 44},
			new Country {ShortName = "Bahrain", Alpha2 = "BH", Alpha3 = "BHR", NumericCode = 48},
			new Country {ShortName = "Bangladesh", Alpha2 = "BD", Alpha3 = "BGD", NumericCode = 50},
			new Country {ShortName = "Barbados", Alpha2 = "BB", Alpha3 = "BRB", NumericCode = 52},
			new Country {ShortName = "Belarus", Alpha2 = "BY", Alpha3 = "BLR", NumericCode = 112},
			new Country {ShortName = "Belgium", Alpha2 = "BE", Alpha3 = "BEL", NumericCode = 56},
			new Country {ShortName = "Belize", Alpha2 = "BZ", Alpha3 = "BLZ", NumericCode = 84},
			new Country {ShortName = "Benin", Alpha2 = "BJ", Alpha3 = "BEN", NumericCode = 204},
			new Country {ShortName = "Bermuda", Alpha2 = "BM", Alpha3 = "BMU", NumericCode = 60},
			new Country {ShortName = "Bhutan", Alpha2 = "BT", Alpha3 = "BTN", NumericCode = 64},
			new Country {ShortName = "Bolivia (Plurinational State of)", Alpha2 = "BO", Alpha3 = "BOL", NumericCode = 68},
			new Country {ShortName = "Bonaire, Sint Eustatius and Saba", Alpha2 = "BQ", Alpha3 = "BES", NumericCode = 535},
			new Country {ShortName = "Bosnia and Herzegovina", Alpha2 = "BA", Alpha3 = "BIH", NumericCode = 70},
			new Country {ShortName = "Botswana", Alpha2 = "BW", Alpha3 = "BWA", NumericCode = 72},
			new Country {ShortName = "Bouvet Island", Alpha2 = "BV", Alpha3 = "BVT", NumericCode = 74},
			new Country {ShortName = "Brazil", Alpha2 = "BR", Alpha3 = "BRA", NumericCode = 76},
			new Country {ShortName = "British Indian Ocean Territory", Alpha2 = "IO", Alpha3 = "IOT", NumericCode = 86},
			new Country {ShortName = "Brunei Darussalam", Alpha2 = "BN", Alpha3 = "BRN", NumericCode = 96},
			new Country {ShortName = "Bulgaria", Alpha2 = "BG", Alpha3 = "BGR", NumericCode = 100},
			new Country {ShortName = "Burkina Faso", Alpha2 = "BF", Alpha3 = "BFA", NumericCode = 854},
			new Country {ShortName = "Burundi", Alpha2 = "BI", Alpha3 = "BDI", NumericCode = 108},
			new Country {ShortName = "Cambodia", Alpha2 = "KH", Alpha3 = "KHM", NumericCode = 116},
			new Country {ShortName = "Cameroon", Alpha2 = "CM", Alpha3 = "CMR", NumericCode = 120},
			new Country {ShortName = "Canada", Alpha2 = "CA", Alpha3 = "CAN", NumericCode = 124},
			new Country {ShortName = "Cabo Verde", Alpha2 = "CV", Alpha3 = "CPV", NumericCode = 132},
			new Country {ShortName = "Cayman Islands", Alpha2 = "KY", Alpha3 = "CYM", NumericCode = 136},
			new Country {ShortName = "Central African Republic", Alpha2 = "CF", Alpha3 = "CAF", NumericCode = 140},
			new Country {ShortName = "Chad", Alpha2 = "TD", Alpha3 = "TCD", NumericCode = 148},
			new Country {ShortName = "Chile", Alpha2 = "CL", Alpha3 = "CHL", NumericCode = 152},
			new Country {ShortName = "China", Alpha2 = "CN", Alpha3 = "CHN", NumericCode = 156},
			new Country {ShortName = "Christmas Island", Alpha2 = "CX", Alpha3 = "CXR", NumericCode = 162},
			new Country {ShortName = "Cocos (Keeling) Islands", Alpha2 = "CC", Alpha3 = "CCK", NumericCode = 166},
			new Country {ShortName = "Colombia", Alpha2 = "CO", Alpha3 = "COL", NumericCode = 170},
			new Country {ShortName = "Comoros", Alpha2 = "KM", Alpha3 = "COM", NumericCode = 174},
			new Country {ShortName = "Congo", Alpha2 = "CG", Alpha3 = "COG", NumericCode = 178},
			new Country {ShortName = "Congo (Democratic Republic of the)", Alpha2 = "CD", Alpha3 = "COD", NumericCode = 180},
			new Country {ShortName = "Cook Islands", Alpha2 = "CK", Alpha3 = "COK", NumericCode = 184},
			new Country {ShortName = "Costa Rica", Alpha2 = "CR", Alpha3 = "CRI", NumericCode = 188},
			new Country {ShortName = "Côte d'Ivoire", Alpha2 = "CI", Alpha3 = "CIV", NumericCode = 384},
			new Country {ShortName = "Croatia", Alpha2 = "HR", Alpha3 = "HRV", NumericCode = 191},
			new Country {ShortName = "Cuba", Alpha2 = "CU", Alpha3 = "CUB", NumericCode = 192},
			new Country {ShortName = "Curaçao", Alpha2 = "CW", Alpha3 = "CUW", NumericCode = 531},
			new Country {ShortName = "Cyprus", Alpha2 = "CY", Alpha3 = "CYP", NumericCode = 196},
			new Country {ShortName = "Czech Republic", Alpha2 = "CZ", Alpha3 = "CZE", NumericCode = 203},
			new Country {ShortName = "Denmark", Alpha2 = "DK", Alpha3 = "DNK", NumericCode = 208},
			new Country {ShortName = "Djibouti", Alpha2 = "DJ", Alpha3 = "DJI", NumericCode = 262},
			new Country {ShortName = "Dominica", Alpha2 = "DM", Alpha3 = "DMA", NumericCode = 212},
			new Country {ShortName = "Dominican Republic", Alpha2 = "DO", Alpha3 = "DOM", NumericCode = 214},
			new Country {ShortName = "Ecuador", Alpha2 = "EC", Alpha3 = "ECU", NumericCode = 218},
			new Country {ShortName = "Egypt", Alpha2 = "EG", Alpha3 = "EGY", NumericCode = 818},
			new Country {ShortName = "El Salvador", Alpha2 = "SV", Alpha3 = "SLV", NumericCode = 222},
			new Country {ShortName = "Equatorial Guinea", Alpha2 = "GQ", Alpha3 = "GNQ", NumericCode = 226},
			new Country {ShortName = "Eritrea", Alpha2 = "ER", Alpha3 = "ERI", NumericCode = 232},
			new Country {ShortName = "Estonia", Alpha2 = "EE", Alpha3 = "EST", NumericCode = 233},
			new Country {ShortName = "Ethiopia", Alpha2 = "ET", Alpha3 = "ETH", NumericCode = 231},
			new Country {ShortName = "Falkland Islands (Malvinas)", Alpha2 = "FK", Alpha3 = "FLK", NumericCode = 238},
			new Country {ShortName = "Faroe Islands", Alpha2 = "FO", Alpha3 = "FRO", NumericCode = 234},
			new Country {ShortName = "Fiji", Alpha2 = "FJ", Alpha3 = "FJI", NumericCode = 242},
			new Country {ShortName = "Finland", Alpha2 = "FI", Alpha3 = "FIN", NumericCode = 246},
			new Country {ShortName = "France", Alpha2 = "FR", Alpha3 = "FRA", NumericCode = 250},
			new Country {ShortName = "French Guiana", Alpha2 = "GF", Alpha3 = "GUF", NumericCode = 254},
			new Country {ShortName = "French Polynesia", Alpha2 = "PF", Alpha3 = "PYF", NumericCode = 258},
			new Country {ShortName = "French Southern Territories", Alpha2 = "TF", Alpha3 = "ATF", NumericCode = 260},
			new Country {ShortName = "Gabon", Alpha2 = "GA", Alpha3 = "GAB", NumericCode = 266},
			new Country {ShortName = "Gambia", Alpha2 = "GM", Alpha3 = "GMB", NumericCode = 270},
			new Country {ShortName = "Georgia", Alpha2 = "GE", Alpha3 = "GEO", NumericCode = 268},
			new Country {ShortName = "Germany", Alpha2 = "DE", Alpha3 = "DEU", NumericCode = 276},
			new Country {ShortName = "Ghana", Alpha2 = "GH", Alpha3 = "GHA", NumericCode = 288},
			new Country {ShortName = "Gibraltar", Alpha2 = "GI", Alpha3 = "GIB", NumericCode = 292},
			new Country {ShortName = "Greece", Alpha2 = "GR", Alpha3 = "GRC", NumericCode = 300},
			new Country {ShortName = "Greenland", Alpha2 = "GL", Alpha3 = "GRL", NumericCode = 304},
			new Country {ShortName = "Grenada", Alpha2 = "GD", Alpha3 = "GRD", NumericCode = 308},
			new Country {ShortName = "Guadeloupe", Alpha2 = "GP", Alpha3 = "GLP", NumericCode = 312},
			new Country {ShortName = "Guam", Alpha2 = "GU", Alpha3 = "GUM", NumericCode = 316},
			new Country {ShortName = "Guatemala", Alpha2 = "GT", Alpha3 = "GTM", NumericCode = 320},
			new Country {ShortName = "Guernsey", Alpha2 = "GG", Alpha3 = "GGY", NumericCode = 831},
			new Country {ShortName = "Guinea", Alpha2 = "GN", Alpha3 = "GIN", NumericCode = 324},
			new Country {ShortName = "Guinea-Bissau", Alpha2 = "GW", Alpha3 = "GNB", NumericCode = 624},
			new Country {ShortName = "Guyana", Alpha2 = "GY", Alpha3 = "GUY", NumericCode = 328},
			new Country {ShortName = "Haiti", Alpha2 = "HT", Alpha3 = "HTI", NumericCode = 332},
			new Country {ShortName = "Heard Island and McDonald Islands", Alpha2 = "HM", Alpha3 = "HMD", NumericCode = 334},
			new Country {ShortName = "Holy See", Alpha2 = "VA", Alpha3 = "VAT", NumericCode = 336},
			new Country {ShortName = "Honduras", Alpha2 = "HN", Alpha3 = "HND", NumericCode = 340},
			new Country {ShortName = "Hong Kong", Alpha2 = "HK", Alpha3 = "HKG", NumericCode = 344},
			new Country {ShortName = "Hungary", Alpha2 = "HU", Alpha3 = "HUN", NumericCode = 348},
			new Country {ShortName = "Iceland", Alpha2 = "IS", Alpha3 = "ISL", NumericCode = 352},
			new Country {ShortName = "India", Alpha2 = "IN", Alpha3 = "IND", NumericCode = 356},
			new Country {ShortName = "Indonesia", Alpha2 = "ID", Alpha3 = "IDN", NumericCode = 360},
			new Country {ShortName = "Iran (Islamic Republic of)", Alpha2 = "IR", Alpha3 = "IRN", NumericCode = 364},
			new Country {ShortName = "Iraq", Alpha2 = "IQ", Alpha3 = "IRQ", NumericCode = 368},
			new Country {ShortName = "Ireland", Alpha2 = "IE", Alpha3 = "IRL", NumericCode = 372},
			new Country {ShortName = "Isle of Man", Alpha2 = "IM", Alpha3 = "IMN", NumericCode = 833},
			new Country {ShortName = "Israel", Alpha2 = "IL", Alpha3 = "ISR", NumericCode = 376},
			new Country {ShortName = "Italy", Alpha2 = "IT", Alpha3 = "ITA", NumericCode = 380},
			new Country {ShortName = "Jamaica", Alpha2 = "JM", Alpha3 = "JAM", NumericCode = 388},
			new Country {ShortName = "Japan", Alpha2 = "JP", Alpha3 = "JPN", NumericCode = 392},
			new Country {ShortName = "Jersey", Alpha2 = "JE", Alpha3 = "JEY", NumericCode = 832},
			new Country {ShortName = "Jordan", Alpha2 = "JO", Alpha3 = "JOR", NumericCode = 400},
			new Country {ShortName = "Kazakhstan", Alpha2 = "KZ", Alpha3 = "KAZ", NumericCode = 398},
			new Country {ShortName = "Kenya", Alpha2 = "KE", Alpha3 = "KEN", NumericCode = 404},
			new Country {ShortName = "Kiribati", Alpha2 = "KI", Alpha3 = "KIR", NumericCode = 296},
			new Country {ShortName = "Korea (Democratic People's Republic of)", Alpha2 = "KP", Alpha3 = "PRK", NumericCode = 408},
			new Country {ShortName = "Korea (Republic of)", Alpha2 = "KR", Alpha3 = "KOR", NumericCode = 410},
			new Country {ShortName = "Kosovo", Alpha2 ="XK", Alpha3 = "XKX", NumericCode = 0},
			new Country {ShortName = "Kuwait", Alpha2 = "KW", Alpha3 = "KWT", NumericCode = 414},
			new Country {ShortName = "Kyrgyzstan", Alpha2 = "KG", Alpha3 = "KGZ", NumericCode = 417},
			new Country {ShortName = "Lao People's Democratic Republic", Alpha2 = "LA", Alpha3 = "LAO", NumericCode = 418},
			new Country {ShortName = "Latvia", Alpha2 = "LV", Alpha3 = "LVA", NumericCode = 428},
			new Country {ShortName = "Lebanon", Alpha2 = "LB", Alpha3 = "LBN", NumericCode = 422},
			new Country {ShortName = "Lesotho", Alpha2 = "LS", Alpha3 = "LSO", NumericCode = 426},
			new Country {ShortName = "Liberia", Alpha2 = "LR", Alpha3 = "LBR", NumericCode = 430},
			new Country {ShortName = "Libya", Alpha2 = "LY", Alpha3 = "LBY", NumericCode = 434},
			new Country {ShortName = "Liechtenstein", Alpha2 = "LI", Alpha3 = "LIE", NumericCode = 438},
			new Country {ShortName = "Lithuania", Alpha2 = "LT", Alpha3 = "LTU", NumericCode = 440},
			new Country {ShortName = "Luxembourg", Alpha2 = "LU", Alpha3 = "LUX", NumericCode = 442},
			new Country {ShortName = "Macao", Alpha2 = "MO", Alpha3 = "MAC", NumericCode = 446},
			new Country {ShortName = "Macedonia (the former Yugoslav Republic of)", Alpha2 = "MK", Alpha3 = "MKD", NumericCode = 807},
			new Country {ShortName = "Madagascar", Alpha2 = "MG", Alpha3 = "MDG", NumericCode = 450},
			new Country {ShortName = "Malawi", Alpha2 = "MW", Alpha3 = "MWI", NumericCode = 454},
			new Country {ShortName = "Malaysia", Alpha2 = "MY", Alpha3 = "MYS", NumericCode = 458},
			new Country {ShortName = "Maldives", Alpha2 = "MV", Alpha3 = "MDV", NumericCode = 462},
			new Country {ShortName = "Mali", Alpha2 = "ML", Alpha3 = "MLI", NumericCode = 466},
			new Country {ShortName = "Malta", Alpha2 = "MT", Alpha3 = "MLT", NumericCode = 470},
			new Country {ShortName = "Marshall Islands", Alpha2 = "MH", Alpha3 = "MHL", NumericCode = 584},
			new Country {ShortName = "Martinique", Alpha2 = "MQ", Alpha3 = "MTQ", NumericCode = 474},
			new Country {ShortName = "Mauritania", Alpha2 = "MR", Alpha3 = "MRT", NumericCode = 478},
			new Country {ShortName = "Mauritius", Alpha2 = "MU", Alpha3 = "MUS", NumericCode = 480},
			new Country {ShortName = "Mayotte", Alpha2 = "YT", Alpha3 = "MYT", NumericCode = 175},
			new Country {ShortName = "Mexico", Alpha2 = "MX", Alpha3 = "MEX", NumericCode = 484},
			new Country {ShortName = "Micronesia (Federated States of)", Alpha2 = "FM", Alpha3 = "FSM", NumericCode = 583},
			new Country {ShortName = "Moldova (Republic of)", Alpha2 = "MD", Alpha3 = "MDA", NumericCode = 498},
			new Country {ShortName = "Monaco", Alpha2 = "MC", Alpha3 = "MCO", NumericCode = 492},
			new Country {ShortName = "Mongolia", Alpha2 = "MN", Alpha3 = "MNG", NumericCode = 496},
			new Country {ShortName = "Montenegro", Alpha2 = "ME", Alpha3 = "MNE", NumericCode = 499},
			new Country {ShortName = "Montserrat", Alpha2 = "MS", Alpha3 = "MSR", NumericCode = 500},
			new Country {ShortName = "Morocco", Alpha2 = "MA", Alpha3 = "MAR", NumericCode = 504},
			new Country {ShortName = "Mozambique", Alpha2 = "MZ", Alpha3 = "MOZ", NumericCode = 508},
			new Country {ShortName = "Myanmar", Alpha2 = "MM", Alpha3 = "MMR", NumericCode = 104},
			new Country {ShortName = "Namibia", Alpha2 = "NA", Alpha3 = "NAM", NumericCode = 516},
			new Country {ShortName = "Nauru", Alpha2 = "NR", Alpha3 = "NRU", NumericCode = 520},
			new Country {ShortName = "Nepal", Alpha2 = "NP", Alpha3 = "NPL", NumericCode = 524},
			new Country {ShortName = "Netherlands", Alpha2 = "NL", Alpha3 = "NLD", NumericCode = 528},
			new Country {ShortName = "New Caledonia", Alpha2 = "NC", Alpha3 = "NCL", NumericCode = 540},
			new Country {ShortName = "New Zealand", Alpha2 = "NZ", Alpha3 = "NZL", NumericCode = 554},
			new Country {ShortName = "Nicaragua", Alpha2 = "NI", Alpha3 = "NIC", NumericCode = 558},
			new Country {ShortName = "Niger", Alpha2 = "NE", Alpha3 = "NER", NumericCode = 562},
			new Country {ShortName = "Nigeria", Alpha2 = "NG", Alpha3 = "NGA", NumericCode = 566},
			new Country {ShortName = "Niue", Alpha2 = "NU", Alpha3 = "NIU", NumericCode = 570},
			new Country {ShortName = "Norfolk Island", Alpha2 = "NF", Alpha3 = "NFK", NumericCode = 574},
			new Country {ShortName = "Northern Mariana Islands", Alpha2 = "MP", Alpha3 = "MNP", NumericCode = 580},
			new Country {ShortName = "Norway", Alpha2 = "NO", Alpha3 = "NOR", NumericCode = 578},
			new Country {ShortName = "Oman", Alpha2 = "OM", Alpha3 = "OMN", NumericCode = 512},
			new Country {ShortName = "Pakistan", Alpha2 = "PK", Alpha3 = "PAK", NumericCode = 586},
			new Country {ShortName = "Palau", Alpha2 = "PW", Alpha3 = "PLW", NumericCode = 585},
			new Country {ShortName = "Palestine, State of", Alpha2 = "PS", Alpha3 = "PSE", NumericCode = 275},
			new Country {ShortName = "Panama", Alpha2 = "PA", Alpha3 = "PAN", NumericCode = 591},
			new Country {ShortName = "Papua New Guinea", Alpha2 = "PG", Alpha3 = "PNG", NumericCode = 598},
			new Country {ShortName = "Paraguay", Alpha2 = "PY", Alpha3 = "PRY", NumericCode = 600},
			new Country {ShortName = "Peru", Alpha2 = "PE", Alpha3 = "PER", NumericCode = 604},
			new Country {ShortName = "Philippines", Alpha2 = "PH", Alpha3 = "PHL", NumericCode = 608},
			new Country {ShortName = "Pitcairn", Alpha2 = "PN", Alpha3 = "PCN", NumericCode = 612},
			new Country {ShortName = "Poland", Alpha2 = "PL", Alpha3 = "POL", NumericCode = 616},
			new Country {ShortName = "Portugal", Alpha2 = "PT", Alpha3 = "PRT", NumericCode = 620},
			new Country {ShortName = "Puerto Rico", Alpha2 = "PR", Alpha3 = "PRI", NumericCode = 630},
			new Country {ShortName = "Qatar", Alpha2 = "QA", Alpha3 = "QAT", NumericCode = 634},
			new Country {ShortName = "Réunion", Alpha2 = "RE", Alpha3 = "REU", NumericCode = 638},
			new Country {ShortName = "Romania", Alpha2 = "RO", Alpha3 = "ROU", NumericCode = 642},
			new Country {ShortName = "Russian Federation", Alpha2 = "RU", Alpha3 = "RUS", NumericCode = 643},
			new Country {ShortName = "Rwanda", Alpha2 = "RW", Alpha3 = "RWA", NumericCode = 646},
			new Country {ShortName = "Saint Barthélemy", Alpha2 = "BL", Alpha3 = "BLM", NumericCode = 652},
			new Country {ShortName = "Saint Helena, Ascension and Tristan da Cunha", Alpha2 = "SH", Alpha3 = "SHN", NumericCode = 654},
			new Country {ShortName = "Saint Kitts and Nevis", Alpha2 = "KN", Alpha3 = "KNA", NumericCode = 659},
			new Country {ShortName = "Saint Lucia", Alpha2 = "LC", Alpha3 = "LCA", NumericCode = 662},
			new Country {ShortName = "Saint Martin (French part)", Alpha2 = "MF", Alpha3 = "MAF", NumericCode = 663},
			new Country {ShortName = "Saint Pierre and Miquelon", Alpha2 = "PM", Alpha3 = "SPM", NumericCode = 666},
			new Country {ShortName = "Saint Vincent and the Grenadines", Alpha2 = "VC", Alpha3 = "VCT", NumericCode = 670},
			new Country {ShortName = "Samoa", Alpha2 = "WS", Alpha3 = "WSM", NumericCode = 882},
			new Country {ShortName = "San Marino", Alpha2 = "SM", Alpha3 = "SMR", NumericCode = 674},
			new Country {ShortName = "Sao Tome and Principe", Alpha2 = "ST", Alpha3 = "STP", NumericCode = 678},
			new Country {ShortName = "Saudi Arabia", Alpha2 = "SA", Alpha3 = "SAU", NumericCode = 682},
			new Country {ShortName = "Senegal", Alpha2 = "SN", Alpha3 = "SEN", NumericCode = 686},
			new Country {ShortName = "Serbia", Alpha2 = "RS", Alpha3 = "SRB", NumericCode = 688},
			new Country {ShortName = "Seychelles", Alpha2 = "SC", Alpha3 = "SYC", NumericCode = 690},
			new Country {ShortName = "Sierra Leone", Alpha2 = "SL", Alpha3 = "SLE", NumericCode = 694},
			new Country {ShortName = "Singapore", Alpha2 = "SG", Alpha3 = "SGP", NumericCode = 702},
			new Country {ShortName = "Sint Maarten (Dutch part)", Alpha2 = "SX", Alpha3 = "SXM", NumericCode = 534},
			new Country {ShortName = "Slovakia", Alpha2 = "SK", Alpha3 = "SVK", NumericCode = 703},
			new Country {ShortName = "Slovenia", Alpha2 = "SI", Alpha3 = "SVN", NumericCode = 705},
			new Country {ShortName = "Solomon Islands", Alpha2 = "SB", Alpha3 = "SLB", NumericCode = 90},
			new Country {ShortName = "Somalia", Alpha2 = "SO", Alpha3 = "SOM", NumericCode = 706},
			new Country {ShortName = "South Africa", Alpha2 = "ZA", Alpha3 = "ZAF", NumericCode = 710},
			new Country {ShortName = "South Georgia and the South Sandwich Islands", Alpha2 = "GS", Alpha3 = "SGS", NumericCode = 239},
			new Country {ShortName = "South Sudan", Alpha2 = "SS", Alpha3 = "SSD", NumericCode = 728},
			new Country {ShortName = "Spain", Alpha2 = "ES", Alpha3 = "ESP", NumericCode = 724},
			new Country {ShortName = "Sri Lanka", Alpha2 = "LK", Alpha3 = "LKA", NumericCode = 144},
			new Country {ShortName = "Sudan", Alpha2 = "SD", Alpha3 = "SDN", NumericCode = 729},
			new Country {ShortName = "Suriname", Alpha2 = "SR", Alpha3 = "SUR", NumericCode = 740},
			new Country {ShortName = "Svalbard and Jan Mayen", Alpha2 = "SJ", Alpha3 = "SJM", NumericCode = 744},
			new Country {ShortName = "Swaziland", Alpha2 = "SZ", Alpha3 = "SWZ", NumericCode = 748},
			new Country {ShortName = "Sweden", Alpha2 = "SE", Alpha3 = "SWE", NumericCode = 752},
			new Country {ShortName = "Switzerland", Alpha2 = "CH", Alpha3 = "CHE", NumericCode = 756},
			new Country {ShortName = "Syrian Arab Republic", Alpha2 = "SY", Alpha3 = "SYR", NumericCode = 760},
			new Country {ShortName = "Taiwan, Province of China", Alpha2 = "TW", Alpha3 = "TWN", NumericCode = 158},
			new Country {ShortName = "Tajikistan", Alpha2 = "TJ", Alpha3 = "TJK", NumericCode = 762},
			new Country {ShortName = "Tanzania, United Republic of", Alpha2 = "TZ", Alpha3 = "TZA", NumericCode = 834},
			new Country {ShortName = "Thailand", Alpha2 = "TH", Alpha3 = "THA", NumericCode = 764},
			new Country {ShortName = "Timor-Leste", Alpha2 = "TL", Alpha3 = "TLS", NumericCode = 626},
			new Country {ShortName = "Togo", Alpha2 = "TG", Alpha3 = "TGO", NumericCode = 768},
			new Country {ShortName = "Tokelau", Alpha2 = "TK", Alpha3 = "TKL", NumericCode = 772},
			new Country {ShortName = "Tonga", Alpha2 = "TO", Alpha3 = "TON", NumericCode = 776},
			new Country {ShortName = "Trinidad and Tobago", Alpha2 = "TT", Alpha3 = "TTO", NumericCode = 780},
			new Country {ShortName = "Tunisia", Alpha2 = "TN", Alpha3 = "TUN", NumericCode = 788},
			new Country {ShortName = "Turkey", Alpha2 = "TR", Alpha3 = "TUR", NumericCode = 792},
			new Country {ShortName = "Turkmenistan", Alpha2 = "TM", Alpha3 = "TKM", NumericCode = 795},
			new Country {ShortName = "Turks and Caicos Islands", Alpha2 = "TC", Alpha3 = "TCA", NumericCode = 796},
			new Country {ShortName = "Tuvalu", Alpha2 = "TV", Alpha3 = "TUV", NumericCode = 798},
			new Country {ShortName = "Uganda", Alpha2 = "UG", Alpha3 = "UGA", NumericCode = 800},
			new Country {ShortName = "Ukraine", Alpha2 = "UA", Alpha3 = "UKR", NumericCode = 804},
			new Country {ShortName = "United Arab Emirates", Alpha2 = "AE", Alpha3 = "ARE", NumericCode = 784},
			new Country {ShortName = "United Kingdom of Great Britain and Northern Ireland", Alpha2 = "GB", Alpha3 = "GBR", NumericCode = 826},
			new Country {ShortName = "United States of America", Alpha2 = "US", Alpha3 = "USA", NumericCode = 840},
			new Country {ShortName = "United States Minor Outlying Islands", Alpha2 = "UM", Alpha3 = "UMI", NumericCode = 581},
			new Country {ShortName = "Uruguay", Alpha2 = "UY", Alpha3 = "URY", NumericCode = 858},
			new Country {ShortName = "Uzbekistan", Alpha2 = "UZ", Alpha3 = "UZB", NumericCode = 860},
			new Country {ShortName = "Vanuatu", Alpha2 = "VU", Alpha3 = "VUT", NumericCode = 548},
			new Country {ShortName = "Venezuela (Bolivarian Republic of)", Alpha2 = "VE", Alpha3 = "VEN", NumericCode = 862},
			new Country {ShortName = "Viet Nam", Alpha2 = "VN", Alpha3 = "VNM", NumericCode = 704},
			new Country {ShortName = "Virgin Islands (British)", Alpha2 = "VG", Alpha3 = "VGB", NumericCode = 92},
			new Country {ShortName = "Virgin Islands (U.S.)", Alpha2 = "VI", Alpha3 = "VIR", NumericCode = 850},
			new Country {ShortName = "Wallis and Futuna", Alpha2 = "WF", Alpha3 = "WLF", NumericCode = 876},
			new Country {ShortName = "Western Sahara", Alpha2 = "EH", Alpha3 = "ESH", NumericCode = 732},
			new Country {ShortName = "Yemen", Alpha2 = "YE", Alpha3 = "YEM", NumericCode = 887},
			new Country {ShortName = "Zambia", Alpha2 = "ZM", Alpha3 = "ZMB", NumericCode = 894},
			new Country {ShortName = "Zimbabwe", Alpha2 = "ZW", Alpha3 = "ZWE", NumericCode = 716},
		};
		#endregion

		public Configuration()
		{
			AutomaticMigrationsEnabled = false;
		}

		protected override void Seed(DebReg.Data.DebRegContext context)
		{
			//  This method will be called after migrating to the latest version.

			//  You can use the DbSet<T>.AddOrUpdate() helper extension method 
			//  to avoid creating duplicate seed data. E.g.
			//
			//    context.People.AddOrUpdate(
			//      p => p.FullName,
			//      new Person { FullName = "Andrew Peters" },
			//      new Person { FullName = "Brice Lambson" },
			//      new Person { FullName = "Rowan Miller" }
			//    );
			//

			//if (System.Diagnostics.Debugger.IsAttached == false)
			//    System.Diagnostics.Debugger.Launch();

			//Create Countries

			foreach (var country in countries)
			{
				country.Id = Guid.NewGuid();
				context.Countries.AddOrUpdate(c => c.NumericCode, country);
			}
			context.SaveChanges();

			// Create Address
			#region CREATE ADDRESS
			Address address = new Address
			{
				Id = Guid.NewGuid(),
				StreetAddress1 = "Address of your society",
				PostalCode = "9999",
				City = "City",
				Country = "Country"
			};
			context.Addresses.AddOrUpdate(a => a.StreetAddress1, address);
			context.SaveChanges();

			#endregion

			// Create organization
			#region CREATE ORGANIZATION

			Organization organization = new Organization
			{
				Id = Guid.NewGuid(),
				Name = "Name of your society",
				Abbreviation = "Abbreviation of your society",
				University = false,
				Status = OrganizationStatus.Unknown,
				SMTPHostConfiguration = new SMTPHostConfiguration
				{
					Id = Guid.NewGuid(),
					Host = "smtp.server.com",
					Port = 587,
					SSL = true,
					Username = "registration@yourtournament.com",
					Password = "Pa$$w0rd",
					FromAddress = "registration@yourtournament.com"
                },
				AddressId = address.Id,
			};
			context.Organizations.AddOrUpdate(o => o.Name, organization);
			context.SaveChanges();

			#endregion

			// Create Currency
			#region CREATE CURRENCY
			Currency currency = new Currency
			{
				Id = Guid.NewGuid(),
				Name = "Euro",
				Symbol = "€"
			};
			context.Currencies.AddOrUpdate(c => c.Name, currency);
			context.SaveChanges();
			#endregion

			// Create Bank Address
			#region CREATE BANK ADDRESS
			Address bankAddress = new Address
			{
				Id = Guid.NewGuid(),
				StreetAddress1 = "Bank Address",
				PostalCode = "9999",
				City = "City",
				Country = "Country"
			};
			context.Addresses.AddOrUpdate(a => a.StreetAddress1, bankAddress);
			context.SaveChanges();
			#endregion


			// Create Bank Account
			#region CREATE BANK ACCOUNT
			BankAccount bankAccount = new BankAccount
			{
				Id = Guid.NewGuid(),
				OrganizationId = organization.Id,
				BankName = "Bank Ltd.",
				Bic = "BANKATW1",
				BankAddressId = bankAddress.Id,
				Iban = "AT999999999999999999"
			};
			context.BankAccounts.AddOrUpdate(ba => ba.Iban, bankAccount);
			context.SaveChanges();
			#endregion

			// Seed tournaments
			#region CREATE Tournament

			TimeZoneInfo timeZone = TimeZoneInfo.FindSystemTimeZoneById("W. Europe Standard Time");
			Tournament tournament = new Tournament
			{
				Id = Guid.NewGuid(),
				Name = "Name of Tournament",
				HostingOrganization = organization,
				HostingOrganizationID = organization.Id,
				Start = TimeZoneInfo.ConvertTimeToUtc(new DateTime(2015, 8, 2, 16, 0, 0), timeZone),
				End = TimeZoneInfo.ConvertTimeToUtc(new DateTime(2015, 8, 8, 11, 0, 0), timeZone),
				Location = "City",
				RegistrationStart = TimeZoneInfo.ConvertTimeToUtc(new DateTime(2015, 2, 12, 14, 0, 0), timeZone),
				RegistrationEnd = TimeZoneInfo.ConvertTimeToUtc(new DateTime(2015, 2, 14, 14, 0, 0), timeZone),
				TeamCap = 220,
				AdjudicatorCap = 145,
				TeamSize = 2,
				AdjucatorSubtract = 1,
				UniversityRequired = true,
				CurrencyId = currency.Id,
				FinanceEMail = "registration@yourtournament.com",
				BankAccountId = bankAccount.Id,
				MoneyTransferLink = "https://transferwise.com/u/ab9999",
				MoneyTransferLinkCaption = "We encourage you to save on banking fees, and sign up with our partner TransferWise. They will contribute financially to our tournament for each new sign up using the following link - so feel very invited to use it!",
				PaymentReference = "VEUDC {0} {1}", // {0} abbreviation of society {1} unique booking code
				TermsConditions = "All payments are subject to the terms and conditions linked below.  You must include the payment reference in each transaction. Otherwise we may be unable to locate your payments. Please mind that the tournament administration will cut and reallocate your slots if full payment has not been received by the due date. All transaction fees are on the expense of the sender.",
				TermsConditionsLink = "http://www.server.com/somedocument.pdf",
				FixedTeamNames = true // team names are derived from abbreviations and cannot be changed
				//PaymentsDueDate = new DateTime(2015, 3, 27)
			};

#if DEBUG
			tournament.RegistrationEnd = tournament.Start;
#endif

			context.Tournaments.AddOrUpdate(t => t.Name, tournament);
			context.SaveChanges();

			#endregion


			// Seed Products
			#region CREATE PRODUCTS

			Product[] Products = new Product[] {
			new Product {
				Name = "Team Registration Fee",
				Description = "",
				Price = 640,
				VatRate = 0.2M
			},
			new Product {
				Name = "Adjudicator Registration Fee",
				Description="",
				Price = 320,
				VatRate = 0.2M
			}
		};

			foreach (var product in Products)
			{
				product.Id = Guid.NewGuid();
				product.TournamentId = tournament.Id;
				//product.Tournament = tournament;
				context.Products.AddOrUpdate(p => p.Name, product);
				context.SaveChanges();
			}

			#endregion

			// Associate Products with Tournament
			#region ASSOCIATE PRODUCTS WITH TOURNAMENT
			tournament.TeamProductId = Products[0].Id;
			tournament.TeamProduct = Products[0];
			tournament.AdjudicatorProductId = Products[1].Id;
			tournament.AdjudicatorProduct = Products[1];
			context.Tournaments.AddOrUpdate(t => t.Name, tournament);
			context.SaveChanges();
			#endregion

			// Create roles
			#region CREATE ROLES

			var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
			IdentityRole identityRole;

			foreach (var item in Enum.GetNames(typeof(OrganizationRole)))
			{
				if (!roleManager.RoleExists(item))
				{
					identityRole = new IdentityRole(item);
					roleManager.Create(identityRole);
				}
			}

			foreach (var item in Enum.GetNames(typeof(TournamentRole)))
			{
				identityRole = new IdentityRole(item);
				roleManager.Create(identityRole);
			}

			identityRole = new IdentityRole("TournamentManager");
			roleManager.Create(identityRole);

			#endregion

			// Create users

			#region CREATE USERS

			PasswordHasher hasher = new PasswordHasher();

			#region user data
			User[] Users = new User[] {
				new User {
					Email = "registration@yourtournament.com",
					UserName = "registration@yourtournament.com",
					FirstName = "Registration",
					LastName = "Registration",
					SponsoringOrganizationId = organization.Id,
					PhoneNumber = "+99555123456",
				//	PasswordHash = hasher.HashPassword("Pa$$w0rd")
				}
				
			};
			#endregion

			TournamentRole[] tournamentRoles = new TournamentRole[] {
				TournamentRole.FinanceManager,
				TournamentRole.OrganizationApprover,
				TournamentRole.SlotManager
			};

			var userManager = new DebRegUserManager(new ApplicationUserStore<User>(context));

			foreach (var user in Users)
			{

				var dbUser = userManager.FindByEmail(user.Email);

				if (dbUser == null)
				{
					userManager.Create(user, "Pa$$w0rd");
					dbUser = userManager.FindByEmail(user.Email);

					//user.Id = Guid.NewGuid().ToString();
					//user.SecurityStamp = Guid.NewGuid().ToString();
				}
				else
				{
					userManager.Update(user);
				}
				dbUser = userManager.FindByEmail(user.Email);


				// Associate user with organization

				var association = new OrganizationUser
				{
					OrganizationId = organization.Id,
					UserId = dbUser.Id,
					Role = OrganizationRole.Delegate
				};

				context.OrganizationUsers.AddOrUpdate(association);
				context.SaveChanges();

				// Associate user with tournament

				foreach (var role in tournamentRoles)
				{
					TournamentUserRole tournamentRole = new TournamentUserRole
					{
						// Id = Guid.NewGuid(),
						TournamentId = tournament.Id,
						UserId = dbUser.Id,
						Role = role
					};

					context.TournamentUserRoles.AddOrUpdate(tournamentRole);
					context.SaveChanges();
				}

			}
			#endregion

			// Create User Properties

			#region CREATE USER PROPERTIES

			{
				UserProperty property;
				UserTournamentProperty tournamentProperty;
				List<UserPropertyOption> propertyOptions = new List<UserPropertyOption>();

				#region Date of Birth
				property = new UserProperty
				{
					Id = Guid.NewGuid(),
					Order = 4,
					Name = "Date of Birth",
					Type = PropertyType.Date
				};
				context.UserProperties.AddOrUpdate(p => p.Name, property);
				context.SaveChanges();

				tournamentProperty = MakeUserPropertyRequiredForTournament(context, tournament.Id, property.Id);

				#endregion

				#region Passport Number
				property = new UserProperty
				{
					Id = Guid.NewGuid(),
					Order = 6,
					Name = "Passport Number",
					Type = PropertyType.String
				};
				context.UserProperties.AddOrUpdate(p => p.Name, property);
				context.SaveChanges();

				tournamentProperty = MakeUserPropertyRequiredForTournament(context, tournament.Id, property.Id);
				#endregion

				#region Gender
				property = new UserProperty
				{
					Id = Guid.NewGuid(),
					Order = 10,
					Name = "Gender",
					Description = "With which gender do you identify the most?",
					Type = PropertyType.SingleSelect,
				};


				context.UserProperties.AddOrUpdate(p => p.Name, property);
				context.SaveChanges();

				propertyOptions.Clear();
				propertyOptions.Add(new UserPropertyOption { Order = 10, Name = "Female", UserPropertyId = property.Id, Id = Guid.NewGuid() });
				propertyOptions.Add(new UserPropertyOption { Order = 20, Name = "Male", UserPropertyId = property.Id, Id = Guid.NewGuid() });
				propertyOptions.Add(new UserPropertyOption { Order = 30, Name = "Other", UserPropertyId = property.Id, Id = Guid.NewGuid() });
				propertyOptions.Add(new UserPropertyOption { Order = 40, Name = "Not Revealing", UserPropertyId = property.Id, Id = Guid.NewGuid() });

				foreach (var propertyOption in propertyOptions)
				{
					context.Set<UserPropertyOption>().AddOrUpdate(o => new { o.Order, o.UserPropertyId }, propertyOption);
					context.SaveChanges();
				}

				tournamentProperty = MakeUserPropertyRequiredForTournament(context, tournament.Id, property.Id);
				#endregion

				#region Address
				property = new UserProperty
				{
					Id = Guid.NewGuid(),
					Order = 12,
					Name = "Address",
					Description = "Street address in your home country.",
					Type = PropertyType.Text
				};

				context.UserProperties.AddOrUpdate(p => p.Name, property);
				context.SaveChanges();
				tournamentProperty = MakeUserPropertyRequiredForTournament(context, tournament.Id, property.Id);
				#endregion

				#region Postal Code
				property = new UserProperty
				{
					Id = Guid.NewGuid(),
					Order = 14,
					Name = "Postal Code",
					Type = PropertyType.String
				};

				context.UserProperties.AddOrUpdate(p => p.Name, property);
				context.SaveChanges();
				tournamentProperty = MakeUserPropertyRequiredForTournament(context, tournament.Id, property.Id);
				#endregion

				#region City
				property = new UserProperty
				{
					Id = Guid.NewGuid(),
					Order = 16,
					Name = "City",
					Type = PropertyType.String
				};
				context.UserProperties.AddOrUpdate(p => p.Name, property);
				context.SaveChanges();
				tournamentProperty = MakeUserPropertyRequiredForTournament(context, tournament.Id, property.Id);
				#endregion

				#region Country
				property = new UserProperty
				{
					Id = Guid.NewGuid(),
					Order = 18,
					Name = "Country",
					Type = PropertyType.Country
				};
				context.UserProperties.AddOrUpdate(p => p.Name, property);
				context.SaveChanges();
				tournamentProperty = MakeUserPropertyRequiredForTournament(context, tournament.Id, property.Id);
				#endregion

				#region English language status
				property = new UserProperty
				{
					Id = Guid.NewGuid(),
					Order = 20,
					Name = "English language status",
					Type = PropertyType.SingleSelect,
					Required = false
				};

				context.UserProperties.AddOrUpdate(p => p.Name, property);
				context.SaveChanges();

				propertyOptions.Clear();
				propertyOptions.Add(new UserPropertyOption { Order = 10, Name = "Native speaker", UserPropertyId = property.Id, Id = Guid.NewGuid() });
				propertyOptions.Add(new UserPropertyOption { Order = 20, Name = "Second language", UserPropertyId = property.Id, Id = Guid.NewGuid() });
				propertyOptions.Add(new UserPropertyOption { Order = 30, Name = "Foreign language", UserPropertyId = property.Id, Id = Guid.NewGuid() });

				foreach (var propertyOption in propertyOptions)
				{
					context.Set<UserPropertyOption>().AddOrUpdate(o => new { o.Order, o.UserPropertyId }, propertyOption);
					context.SaveChanges();
				}

				tournamentProperty = new UserTournamentProperty
				{
					TournamentId = tournament.Id,
					UserPropertyId = property.Id,
					Required = false,
					Order = 10
				};

				context.UserTournamentProperties.AddOrUpdate(
					p => new { p.TournamentId, p.UserPropertyId },
					tournamentProperty
				);
				context.SaveChanges();

				tournamentProperty = MakeUserPropertyRequiredForTournament(context, tournament.Id, property.Id);
				#endregion

				#region Dietary requirements



				property = new UserProperty
				{
					Id = Guid.NewGuid(),
					Order = 30,
					Name = "Dietary requirements",
					Type = PropertyType.SingleSelect,
					Required = false
				};

				context.UserProperties.AddOrUpdate(p => p.Name, property);
				context.SaveChanges();

				propertyOptions.Clear();
				propertyOptions.Add(new UserPropertyOption { Order = 10, Name = "Full cuisine (no special requirements)", UserPropertyId = property.Id, Id = Guid.NewGuid() });
				propertyOptions.Add(new UserPropertyOption { Order = 20, Name = "No pork", UserPropertyId = property.Id, Id = Guid.NewGuid() });
				propertyOptions.Add(new UserPropertyOption { Order = 30, Name = "No meat", UserPropertyId = property.Id, Id = Guid.NewGuid() });
				propertyOptions.Add(new UserPropertyOption { Order = 40, Name = "Other (please specify)", UserPropertyId = property.Id, Id = Guid.NewGuid() });

				foreach (var propertyOption in propertyOptions)
				{
					context.Set<UserPropertyOption>().AddOrUpdate(o => new { o.Order, o.UserPropertyId }, propertyOption);
					context.SaveChanges();
				}

				tournamentProperty = new UserTournamentProperty
				{
					TournamentId = tournament.Id,
					UserPropertyId = property.Id,
					Required = false,
					Order = 10
				};

				context.UserTournamentProperties.AddOrUpdate(
					p => new { p.TournamentId, p.UserPropertyId },
					tournamentProperty
				);
				context.SaveChanges();

				tournamentProperty = MakeUserPropertyRequiredForTournament(context, tournament.Id, property.Id);

				#endregion


				#region Special dietary requirements
				property = new UserProperty
				{
					Id = Guid.NewGuid(),
					Order = 40,
					Name = "Special dietary requirements",
					Description = "Only fill in, if you selected 'Other'.",
					Type = PropertyType.Text
				};

				context.UserProperties.AddOrUpdate(p => p.Name, property);
				context.SaveChanges();
				#endregion


				#region Room preference

				property = new UserProperty
				{
					Id = Guid.NewGuid(),
					Name = "Room preference",
					Type = PropertyType.String,
					TournamentSpecific = true,
					Order = 100
				};

				context.UserProperties.AddOrUpdate(p => p.Name, property);
				context.SaveChanges();

				tournamentProperty = new UserTournamentProperty
				{
					TournamentId = tournament.Id,
					UserPropertyId = property.Id,
					Required = false,
					Order = 10
				};

				context.UserTournamentProperties.AddOrUpdate(
					p => new { p.TournamentId, p.UserPropertyId },
					tournamentProperty
				);
				context.SaveChanges();
				#endregion
			}
			#endregion

		}

		private UserTournamentProperty MakeUserPropertyRequiredForTournament(DebReg.Data.DebRegContext context, Guid tournamentId, Guid userPropertyId)
		{
			UserTournamentProperty tournamentProperty;
			tournamentProperty = new UserTournamentProperty
			{
				TournamentId = tournamentId,
				UserPropertyId = userPropertyId,
				Required = true,
			};

			context.UserTournamentProperties.AddOrUpdate(
				p => new { p.TournamentId, p.UserPropertyId },
				tournamentProperty
			);
			context.SaveChanges();
			return tournamentProperty;
		}
	}
}
