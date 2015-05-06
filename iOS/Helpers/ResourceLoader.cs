using System;
using System.Collections.Generic;
using UIKit;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Collections;
using Foundation;

namespace RITMaps.iOS
{
	public class ResourceLoader : IResourceLoader
	{
		public IRITBuilding[] Load (ResourceFile resource)
		{
			UIApplication.SharedApplication.NetworkActivityIndicatorVisible = true;
			var path = NSBundle.MainBundle.PathForResource (Resources.ResourceFileToFileName (resource), "js");
			var jsonStr = File.ReadAllText(path);
			var json = JObject.Parse (jsonStr);
			var buildingsJObj = json ["response"] ["docs"];
			Console.WriteLine (buildingsJObj);
			UIApplication.SharedApplication.NetworkActivityIndicatorVisible = false;
			/*
    [NSURLConnection sendAsynchronousRequest:[[NSURLRequest alloc] initWithURL:resourceLocation] queue:[[NSOperationQueue alloc]init] completionHandler:^(NSURLResponse *response, NSData *data, NSError *error) {
        if (!error) {
            NSDictionary* jsonDictionary = [NSJSONSerialization JSONObjectWithData:data options:0 error:&error];
            if(error)
            {
                UIAlertView* alert = [[UIAlertView alloc] initWithTitle:@"Error parsing JSON file" message:[error description] delegate:self cancelButtonTitle:nil otherButtonTitles:@"Whoops", nil];
                [alert show];
            } else if([resourceName isEqualToString:kMARKER_DATA] || [resourceName isEqualToString:kPOLYGON_DATA])
            {
                NSMutableArray* allBuildings = [NSMutableArray array];
                RITBuilding* building;
                NSMutableArray* jsonBuildings = jsonDictionary[@"response"][@"docs"];
                for (int progress = 0, total = [jsonBuildings count]; progress < total; progress++) {
                    NSDictionary* dictionary = [jsonBuildings objectAtIndex:progress];
                    building = [[RITBuilding alloc] initWithDictionary:dictionary];
                    [allBuildings addObject:building];
                    dispatch_async(dispatch_get_main_queue(), ^{
                        _loadProgressBar.progress = ((float)progress / (float)total);
                    });
                }
                NSSortDescriptor* sortParam = [[NSSortDescriptor alloc] initWithKey:@"title" ascending:YES];
                NSArray* sortParams = @[sortParam];
                [allBuildings sortUsingDescriptors:sortParams];
                [[DataStore sharedStore].allItems addObjectsFromArray:allBuildings];
                [UIApplication sharedApplication].networkActivityIndicatorVisible = NO;
                dispatch_async(dispatch_get_main_queue(), ^{
                    _loadProgressBar.hidden = YES;
                    [_activityIndicator startAnimating];
                    [self performSegueWithIdentifier:@"done" sender:self];
                });
            } else
            {
                NSMutableDictionary* allTags = [NSMutableDictionary dictionary];
                NSArray* arr = jsonDictionary[@"facet_counts"][@"facet_fields"][@"tag"];
                //NSLog(@"%@",arr);
                for (int progress = 0, total = [arr count]; progress < total; progress+=2) {
                    [allTags setObject:[arr objectAtIndex:progress+1] forKey:[arr objectAtIndex:progress]];
                    dispatch_async(dispatch_get_main_queue(), ^{
                        _loadProgressBar.progress = ((float)progress / (float)total);
                    });
                }
                [[DataStore sharedStore].allTags addEntriesFromDictionary:allTags];
            }
        }
    }];
			*/
			return new BuildingAnnotation[]{};
		}
	}
}

