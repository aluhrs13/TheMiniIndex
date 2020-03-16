#!/usr/bin/python
import praw
import requests
import re


reddit = praw.Reddit('bot1')

subreddit = reddit.multireddit("ramones13", "tmilurk")

for comment in subreddit.stream.comments():
    print(comment.body)
    print("---------------------------------------------")
    if re.findall('http[s]?://(?:[a-zA-Z]|[0-9]|[$-_@.&+]|[!*\(\),]|(?:%[0-9a-fA-F][0-9a-fA-F]))+', comment.body):
        urls = re.findall('http[s]?://(?:[a-zA-Z]|[0-9]|[$-_@.&+]|[!*\(\),]|(?:%[0-9a-fA-F][0-9a-fA-F]))+', comment.body)
        for url in urls:
            if(url.endswith(')')):
                parsedUrl = url.split('(')[0].rstrip(')').rstrip(']')
            else:
                parsedUrl = url
            print()
            print()
            print("______________________________________________")
            print("FOUND URL - "+parsedUrl)
            r = requests.get("https://www.theminiindex.com/api/minis/create?url=" + parsedUrl + "&key=<KEY>", verify=False)
            print(r.status_code)
            print(r.content)
            print("______________________________________________")
            print()
            print()

