///* ------------------------------------------------------------------------- *
//thZero.Registry
//Copyright (C) 2021-2021 thZero.com

//<development [at] thzero [dot] com>

//Licensed under the Apache License, Version 2.0 (the "License");
//you may not use this file except in compliance with the License.
//You may obtain a copy of the License at

//	http://www.apache.org/licenses/LICENSE-2.0

//Unless required by applicable law or agreed to in writing, software
//distributed under the License is distributed on an "AS IS" BASIS,
//WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//See the License for the specific language governing permissions and
//limitations under the License.
// * ------------------------------------------------------------------------- */

//using System;
//using System.ComponentModel.DataAnnotations;
//using System.Text.Json.Serialization;

//namespace thZero.Registry.Data
//{
//    public class RegistryData
//    {
//        [Required]
//        [StringLength(25, MinimumLength = 3)]
//        [RegularExpression(@"^([._\-a-zA-Z0-9]*)*$")]
//        public string Address { get; set; }
//        public RegistryAuthentication Authentication { get; set; }
//        public RegistryDns Dns { get; set; }
//        public RegistryGrpc Grpc { get; set; }
//        public RegistryHealthCheck HealthCheck { get; set; }
//        [Required]
//        [StringLength(25, MinimumLength = 3)]
//        [RegularExpression(@"^[a-zA-Z0-9]+(['""._\-a-zA-Z0-9 ]*)*$")]
//        public string Name { get; set; }
//        public string Notes { get; set; }
//        [Required]
//        [Range(0, 65535)]
//        public int? Port { get; set; }
//        public bool Secure { get; set; }
//        public bool Static { get; set; }
//        [Range(0, Int64.MaxValue)]
//        public long Ttl { get; set; }

//        [JsonIgnore]
//        public long Timestamp { get; set; }
//    }

//    public class RegistryAuthentication
//    {
//        [StringLength(36, MinimumLength = 3)]
//        [RegularExpression(@"^([._\-a-zA-Z0-9]*)*$")]
//        public string ApiKey { get; set; }
//    }

//    public class RegistryDns
//    {
//        [StringLength(30, MinimumLength = 3)]
//        [RegularExpression(@"[_\-a-zA-Z0-9]*)*$")]
//        public string Label { get; set; }
//        public bool Local { get; set; }
//        [StringLength(30, MinimumLength = 2)]
//        public string Namespace { get; set; }
//    }

//    public class RegistryGrpc
//    {
//        public bool Enabled { get; set; }
//        public long? Port { get; set; }
//    }

//    public class RegistryHealthCheck
//    {
//        public bool Enabled { get; set; }
//        [StringLength(30, MinimumLength = 3)]
//        [RegularExpression(@"^([._\-a-zA-Z0-9]*)*$")]
//        public string Path { get; set; }
//        [Range(0, Int64.MaxValue)]
//        public long Interval { get; set; }
//        [StringLength(30, MinimumLength = 3)]
//        public string Type { get; set; }
//    }
//}
