-- Execute no SQL Editor do Supabase
create table if not exists public.news_posts (
  id bigint generated always as identity primary key,
  title text not null,
  category text not null,
  summary text not null,
  author text not null,
  created_at timestamptz not null default now()
);

alter table public.news_posts enable row level security;

create policy "Leitura pública de notícias"
on public.news_posts
for select
using (true);

create policy "Inserção pública de notícias"
on public.news_posts
for insert
with check (true);
